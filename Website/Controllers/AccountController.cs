﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Website.Classes;
using DataAccess.Models;
using Website.Repositories;
using Microsoft.AspNetCore.Http;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using Services;
using Services.Classes;
using System.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Website.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<Customer> userManager;
        private readonly IConfiguration configuration;
        private readonly IUnitOfWork unitOfWork;
        private readonly EmailService emailService;
        private readonly IWebHostEnvironment env;

        public AccountController(UserManager<Customer> userManager, IConfiguration configuration, IUnitOfWork unitOfWork, EmailService emailService, IWebHostEnvironment env)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.unitOfWork = unitOfWork;
            this.emailService = emailService;
            this.env = env;
        }




        // ..................................................................................Register.....................................................................
        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult> Register(Account account)
        {
            Customer customer = account.CreateCustomer();

            // Add the new customer to the database
            IdentityResult result = await userManager.CreateAsync(customer, account.Password);


            if (result.Succeeded)
            {
                // Send an email to activate the account
                await SendAccountActivationEmail(customer);


                // Create the new list and add it to the database
                List newList = new List
                {
                    Id = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper(),
                    Name = "My List",
                    Description = string.Empty,
                    CollaborateId = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper()
                };


                unitOfWork.Lists.Add(newList);



                // Set the owner as the first collaborator of the list
                ListCollaborator collaborator = new ListCollaborator
                {
                    CustomerId = customer.Id,
                    ListId = newList.Id,
                    IsOwner = true
                };

                unitOfWork.Collaborators.Add(collaborator);


                // Save all updates to the database
                await unitOfWork.Save();



                // The new customer was successfully added to the database
                return Ok();
            }
            else
            {
                string error = string.Empty;

                if (result.Errors.Count(x => x.Code == "DuplicateEmail") == 1)
                {
                    error = "The email address, \"" + account.Email.ToLower() + ",\" already exists with another Niche Shack account. Please use another email address.";
                }

                return Conflict(error);
            }
        }





        // ..................................................................................Sign In.....................................................................
        [HttpPost]
        [Route("SignIn")]
        public async Task<ActionResult> SignIn(SignIn signIn)
        {
            // Get the customer from the database based on the email address
            Customer customer = await userManager.FindByEmailAsync(signIn.Email);

            if (customer != null && !customer.EmailConfirmed)
            {
                return Unauthorized("This account has not been activated. To activate your account, click on the \"Activate Account\" button in the email that was sent to " + customer.Email + ". ");
            }

            // If the customer is in the database and the password is valid, create claims for the access token
            if (customer != null && await userManager.CheckPasswordAsync(customer, signIn.Password))
            {
                List<Claim> claims = GetClaims(customer, signIn.IsPersistent);

                var tokenData = await GenerateTokenData(customer, claims);
                var customerData = customer.FirstName + "," + customer.LastName + "," + customer.Email + "," + customer.Image;

                CookieOptions cookieOptions = new CookieOptions();

                if (signIn.IsPersistent)
                {
                    cookieOptions = new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddDays(Convert.ToInt32(configuration["TokenValidation:RefreshExpiresInDays"]))
                    };
                }

                Response.Cookies.Append("access", tokenData.AccessToken, cookieOptions);
                Response.Cookies.Append("refresh", tokenData.RefreshToken, cookieOptions);
                Response.Cookies.Append("customer", customerData, cookieOptions);


                return Ok();
            }

            return Conflict("Your password and email do not match. Please try again.");
        }









        // ..................................................................................Get Claims.....................................................................
        private List<Claim> GetClaims(Customer customer, bool isPersistent)
        {
            List<Claim> claims = new List<Claim>()
                {
                    new Claim("acc", "customer"),
                    new Claim(ClaimTypes.NameIdentifier, customer.Id),
                    new Claim(JwtRegisteredClaimNames.Iss, configuration["TokenValidation:Site"]),
                    new Claim(JwtRegisteredClaimNames.Aud, configuration["TokenValidation:Site"])
                };



            if(isPersistent)
            {
                claims.Add(new Claim(ClaimTypes.Expiration, DateTimeOffset.UtcNow.AddDays(Convert.ToInt32(configuration["TokenValidation:RefreshExpiresInDays"])).ToString()));
            }

            return claims;
        }




        // ..................................................................................Activate Account.....................................................................
        [HttpPost]
        [Route("ActivateAccount")]
        public async Task<ActionResult> ActivateAccount(ActivateAccount activateAccount)
        {
            if (activateAccount.Email == null || activateAccount.Token == null)
            {
                return Ok();
            }


            Customer customer = await userManager.FindByEmailAsync(activateAccount.Email);

            if (customer == null)
            {
                return Ok();
            }


            if (customer.EmailConfirmed) return Ok();

            var result = await userManager.ConfirmEmailAsync(customer, activateAccount.Token);

            if (result.Succeeded)
            {
                List<Claim> claims = GetClaims(customer, true);

                return Ok(new
                {
                    tokenData = await GenerateTokenData(customer, claims),
                    customer = new CustomerData
                    {
                        FirstName = customer.FirstName,
                        LastName = customer.LastName,
                        Email = customer.Email,
                        Image = customer.Image
                    }
                });
            }


            return Ok();
        }










        // ..................................................................................Reset Password.....................................................................
        [HttpPost]
        [Route("ResetPassword")]
        public async Task<ActionResult> ResetPassword(ResetPassword resetPassword)
        {
            if (resetPassword.Email != null && resetPassword.Token != null)
            {
                Customer customer = await userManager.FindByEmailAsync(resetPassword.Email);


                if (customer != null)
                {
                    var result = await userManager.ResetPasswordAsync(customer, resetPassword.Token, resetPassword.Password);


                    if (result.Succeeded)
                    {
                        return Ok();
                    }
                }

            }


            return BadRequest();
        }













        // ..................................................................................Change Name.....................................................................
        [HttpPut]
        [Route("ChangeName")]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> ChangeName(UpdatedCustomerName updatedCustomerName)
        {
            // Get the customer from the database based on the customer id from the claims via the access token
            Customer customer = await userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // If the customer is found, update his/her name
            if (customer != null)
            {
                customer.FirstName = updatedCustomerName.FirstName;
                customer.LastName = updatedCustomerName.LastName;

                // Update the name in the database
                IdentityResult result = await userManager.UpdateAsync(customer);

                if (result.Succeeded)
                {
                    // Send a confirmation email that the customer name has been changed
                    if (customer.EmailPrefNameChange == true)
                    {
                        emailService.AddToQueue(EmailType.NameChange, "Name change confirmation", new Recipient
                        {
                            FirstName = customer.FirstName,
                            LastName = customer.LastName,
                            Email = customer.Email
                        }, new EmailProperties { Host = GetHost() });
                    }



                    UpdateCustomerCookie(customer);

                    

                    return Ok();
                }
            }


            return BadRequest();
        }






        void UpdateCustomerCookie(Customer customer)
        {
            Claim expiration = User.FindFirst(ClaimTypes.Expiration);


            CookieOptions cookieOptions = new CookieOptions();

            if (expiration != null)
            {
                cookieOptions = new CookieOptions
                {
                    Expires = DateTimeOffset.Parse(expiration.Value)
                };
            }

            string customerData = customer.FirstName + "," + customer.LastName + "," + customer.Email + "," + customer.Image;

            Response.Cookies.Append("customer", customerData, cookieOptions);
        }



        // ..................................................................................Update Email.....................................................................
        [HttpPut]
        [Route("UpdateEmail")]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> UpdateEmail(UpdatedEmail updatedEmail)
        {
            // Get the customer from the database based on the customer id from the claims via the access token
            Customer customer = await userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);


            // If the customer is found...
            if (customer != null)
            {
                if (!await userManager.CheckPasswordAsync(customer, updatedEmail.Password))
                {
                    return Conflict("Your password and email do not match.");
                }

                string previousEmail = customer.Email;

                // Update the new email in the database
                IdentityResult result = await userManager.ChangeEmailAsync(customer, updatedEmail.Email, updatedEmail.Token);


                // If the update was successful, return ok
                if (result.Succeeded)
                {
                    // Send a confirmation email that the customer email has been changed
                    if (customer.EmailPrefEmailChange == true)
                    {
                        emailService.AddToQueue(EmailType.EmailChange, "Email change confirmation", new Recipient
                        {
                            FirstName = customer.FirstName,
                            LastName = customer.LastName,
                            Email = updatedEmail.Email
                        }, new EmailProperties
                        {
                            Host = GetHost(),
                            Var1 = previousEmail,
                            Var2 = updatedEmail.Email
                        });
                    }


                    UpdateCustomerCookie(customer);
                    return Ok();
                }
                else
                {
                    return Conflict("An error occured due to an invalid email address or invalid token.");
                }

            }

            return BadRequest();
        }








        // ..................................................................................Update Password.....................................................................
        [HttpPut]
        [Route("UpdatePassword")]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> UpdatePassword(UpdatedPassword updatedPassword)
        {
            // Get the customer from the database based on the customer id from the claims via the access token
            Customer customer = await userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // If the customer is found...
            if (customer != null)
            {
                // Update the password in the database
                IdentityResult result = await userManager.ChangePasswordAsync(customer, updatedPassword.CurrentPassword, updatedPassword.NewPassword);


                // If the password was successfully updated, return ok
                if (result.Succeeded)
                {

                    // Send a confirmation email that the customer's password has been changed
                    if (customer.EmailPrefPasswordChange == true)
                    {
                        emailService.AddToQueue(EmailType.PasswordChange, "Password change confirmation", new Recipient
                        {
                            FirstName = customer.FirstName,
                            LastName = customer.LastName,
                            Email = customer.Email
                        }, new EmailProperties { Host = GetHost() });
                    }


                    return Ok();
                }
            }

            return BadRequest();
        }





        // ..................................................................................New Profile Picture.....................................................................
        [HttpPost, DisableRequestSizeLimit]
        [Route("NewProfilePicture")]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> NewImage()
        {
            string imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "images");


            //Get the form data
            IFormFile imageFile = Request.Form.Files["newImage"];
            Request.Form.TryGetValue("width", out StringValues width);
            Request.Form.TryGetValue("height", out StringValues height);
            Request.Form.TryGetValue("cropTop", out StringValues cropTop);
            Request.Form.TryGetValue("cropLeft", out StringValues cropLeft);
            Request.Form.TryGetValue("currentImage", out StringValues currentImage);


            //Convert from image file to bitmap
            Bitmap tempBitmap;
            using (var memoryStream = new MemoryStream())
            {
                await imageFile.CopyToAsync(memoryStream);
                using (var tempImage = System.Drawing.Image.FromStream(memoryStream))
                {
                    tempBitmap = new Bitmap(tempImage);
                }
            }


            //Convert from string to int
            int profilePicWidth = Convert.ToInt32(width);
            int profilePicHeight = Convert.ToInt32(height);
            int profilePicCropTop = Convert.ToInt32(cropTop);
            int profilePicCropLeft = Convert.ToInt32(cropLeft);



            //Scale
            Bitmap scaledBitmap = new Bitmap(profilePicWidth, profilePicHeight);
            Graphics graph = Graphics.FromImage(scaledBitmap);
            graph.DrawImage(tempBitmap, 0, 0, profilePicWidth, profilePicHeight);


            //If the customer currently has an image assigned to their profile
            if (!String.IsNullOrEmpty(currentImage))
            {
                // Delete that customer's current image
                System.IO.File.Delete(imagesFolder + "\\" + currentImage);
            }


            //Crop
            Bitmap croppedBitmap = new Bitmap(300, 300);
            for (int i = 0; i < 300; i++)
            {
                for (int y = 0; y < 300; y++)
                {
                    Color pxlclr = scaledBitmap.GetPixel(profilePicCropLeft + i, profilePicCropTop + y);
                    croppedBitmap.SetPixel(i, y, pxlclr);
                }
            }


            //Create the new image
            string imageName = Guid.NewGuid().ToString("N") + ".png";
            string newImage = Path.Combine(imagesFolder, imageName);
            croppedBitmap.Save(newImage, ImageFormat.Png);


            //Get the customer associated with this profile pic
            string customerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Customer customer = await unitOfWork.Customers.Get(x => x.Id == customerId);


            //Update the customer's profile picture
            customer.Image = imageName;
            unitOfWork.Customers.Update(customer);

            //Save
            await unitOfWork.Save();


            // Send an email
            if (customer.EmailPrefProfilePicChange == true)
            {
                emailService.AddToQueue(EmailType.ProfilePicChange, "Updated profile picture", new Recipient
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email
                }, new EmailProperties { Host = GetHost() });
            }


            return Ok(imageName);
        }




















        // ..................................................................................Edit Profile Picture.....................................................................
        [HttpPost]
        [Route("EditProfilePicture")]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> EditImage(ProfilePic profilePic)
        {
            string imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "images");
            string tempImage = imagesFolder + "\\" + profilePic.Image;


            //Scale
            Bitmap scaledBitmap = new Bitmap(profilePic.Width, profilePic.Height);
            Graphics graph = Graphics.FromImage(scaledBitmap);
            using (Bitmap tempBitmap = new Bitmap(tempImage))
            {
                graph.DrawImage(tempBitmap, 0, 0, profilePic.Width, profilePic.Height);
            }

            // Delete the temp image
            System.IO.File.Delete(tempImage);



            //Crop
            Bitmap croppedBitmap = new Bitmap(300, 300);
            for (int i = 0; i < 300; i++)
            {
                for (int y = 0; y < 300; y++)
                {
                    Color pxlclr = scaledBitmap.GetPixel(profilePic.CropLeft + i, profilePic.CropTop + y);
                    croppedBitmap.SetPixel(i, y, pxlclr);
                }
            }


            //Create the new image
            string imageName = Guid.NewGuid().ToString("N") + ".png";
            string newImage = Path.Combine(imagesFolder, imageName);
            croppedBitmap.Save(newImage, ImageFormat.Png);


            //Get the customer associated with this profile pic
            string customerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Customer customer = await unitOfWork.Customers.Get(x => x.Id == customerId);


            //Update the customer's profile picture
            customer.Image = imageName;
            unitOfWork.Customers.Update(customer);

            //Save
            await unitOfWork.Save();


            // Send an email
            if (customer.EmailPrefProfilePicChange == true)
            {
                emailService.AddToQueue(EmailType.ProfilePicChange, "Updated profile picture", new Recipient
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email
                }, new EmailProperties { Host = GetHost() });
            }



            return Ok(imageName);
        }









        // ..................................................................................Refresh.....................................................................
        [HttpGet]
        [Route("Refresh")]
        public async Task<ActionResult> Refresh()
        {
            string accessToken = GetAccessTokenFromHeader();
            string refresh = Request.Cookies["refresh"];

            if (accessToken != null)
            {
                ClaimsPrincipal principal = GetPrincipalFromToken(accessToken);


                if (principal != null && refresh != null)
                {
                    string customerId = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

                    if (customerId != null)
                    {
                        RefreshToken refreshToken = await unitOfWork.RefreshTokens.Get(x => x.Id == refresh && x.CustomerId == customerId);
                        if (refreshToken != null)
                        {
                            // Remove the refresh token from the database
                            unitOfWork.RefreshTokens.Remove(refreshToken);
                            await unitOfWork.Save();

                            if (DateTime.Compare(DateTime.UtcNow, refreshToken.Expiration) < 0)
                            {
                                Customer customer = await userManager.FindByIdAsync(customerId);

                                // Generate a new token and refresh token
                                return Ok(await GenerateTokenData(customer, principal.Claims));
                            }
                        }
                    }
                }
            }


            return Ok();
        }






        // ..................................................................................Log Out.....................................................................
        [HttpGet]
        [Route("LogOut")]
        public async Task<ActionResult> LogOut()
        {
            string refresh = Request.Cookies["refresh"];

            if (refresh != null)
            {
                RefreshToken refreshToken = await unitOfWork.RefreshTokens.Get(x => x.Id == refresh);

                if (refreshToken != null)
                {
                    unitOfWork.RefreshTokens.Remove(refreshToken);
                    await unitOfWork.Save();
                }

            }

            Response.Cookies.Delete("access");
            Response.Cookies.Delete("refresh");
            Response.Cookies.Delete("customer");

            return NoContent();
        }





        // ..................................................................................Get Customer.....................................................................
        [HttpGet]
        [Route("GetCustomer")]
        public async Task<ActionResult> GetCustomer()
        {
            CustomerData customerData = null;

            if (Request.Cookies["access"] != null)
            {
                Claim claim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (claim != null)
                {
                    Customer customer = await userManager.FindByIdAsync(claim.Value);

                    if (customer != null)
                    {
                        customerData = new CustomerData
                        {
                            FirstName = customer.FirstName,
                            LastName = customer.LastName,
                            Email = customer.Email,
                            Image = customer.Image
                        };
                    }
                }

            }

            if (customerData != null)
            {
                return Ok(customerData);
            }

            return Ok();
        }





        // ..................................................................................Resend Account Activation Email.....................................................................
        [HttpGet]
        [Route("ResendAccountActivationEmail")]
        public async Task<ActionResult> ResendAccountActivationEmail(string email)
        {
            // Get the customer from the database based on the email address
            Customer customer = await userManager.FindByEmailAsync(email);

            if (customer != null)
            {
                await SendAccountActivationEmail(customer);
            }


            return Ok();
        }







        // ..................................................................................Forget Password.....................................................................
        [HttpGet]
        [Route("ForgetPassword")]
        public async Task<ActionResult> ForgetPassword(string email)
        {
            // Get the customer from the database based on the email address
            Customer customer = await userManager.FindByEmailAsync(email);

            if (customer != null)
            {
                await SendResetPasswordEmail(customer);
            }


            return Ok();
        }









        // ..................................................................................New Email.....................................................................
        [HttpGet]
        [Route("NewEmail")]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> NewEmail(string email)
        {
            string customerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Customer customer = await userManager.FindByIdAsync(customerId);


            if (customer != null)
            {
                var result = await userManager.FindByEmailAsync(email);

                if (result != null) return Ok(true);

                string token = await userManager.GenerateChangeEmailTokenAsync(customer, email);
                string host = GetHost();


                emailService.AddToQueue(EmailType.VerifyEmail, "Verify Email", new Recipient
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = email
                }, new EmailProperties
                {
                    Host = host,
                    Var1 = token
                });

                return Ok();
            }


            return BadRequest();
        }








        // ..................................................................................Send Account Activation Email.....................................................................
        private async Task SendAccountActivationEmail(Customer customer)
        {
            string token = await userManager.GenerateEmailConfirmationTokenAsync(customer);
            string host = GetHost();

            emailService.AddToQueue(EmailType.AccountActivation, "Activate your new Niche Shack account", new Recipient
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email
            }, new EmailProperties
            {
                Host = host,
                Link = host + "/activate-account?email=" + HttpUtility.UrlEncode(customer.Email) + "&token=" + HttpUtility.UrlEncode(token)
            });
        }






        // ..................................................................................Send Reset Password Email.....................................................................
        private async Task SendResetPasswordEmail(Customer customer)
        {
            string token = await userManager.GeneratePasswordResetTokenAsync(customer);
            string host = GetHost();

            emailService.AddToQueue(EmailType.ResetPassword, "Reset Password", new Recipient
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email
            }, new EmailProperties
            {
                Host = host,
                Link = host + "/reset-password?email=" + HttpUtility.UrlEncode(customer.Email) + "&token=" + HttpUtility.UrlEncode(token)
            });
        }




        // ..................................................................................Generate Token Data.....................................................................
        private async Task<TokenData> GenerateTokenData(Customer customer, IEnumerable<Claim> claims)
        {
            // Generate the access token
            JwtSecurityToken accessToken = GenerateAccessToken(claims);

            // Generate the refresh token
            RefreshToken refreshToken = await GenerateRefreshToken(customer.Id);


            // Return the token data
            return new TokenData
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
                RefreshToken = refreshToken.Id
            };
        }








        // ..................................................................................Generate Access Token.....................................................................
        private JwtSecurityToken GenerateAccessToken(IEnumerable<Claim> claims)
        {
            return new JwtSecurityToken(
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(configuration["TokenValidation:AccessExpiresInMinutes"])),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenValidation:SigningKey"])), SecurityAlgorithms.HmacSha256),
                claims: claims);
        }










        // ..................................................................................Generate Refresh Token.....................................................................
        private async Task<RefreshToken> GenerateRefreshToken(string customerId)
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);

                RefreshToken refreshToken = new RefreshToken()
                {
                    Id = Convert.ToBase64String(randomNumber),
                    CustomerId = customerId,
                    Expiration = DateTime.UtcNow.AddDays(Convert.ToInt32(configuration["TokenValidation:RefreshExpiresInDays"]))
                };

                // Add to database
                unitOfWork.RefreshTokens.Add(refreshToken);

                await unitOfWork.Save();

                return refreshToken;
            }
        }












        // ..................................................................................Get Principal From Token....................................................................
        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = configuration["TokenValidation:Site"],
                ValidIssuer = configuration["TokenValidation:Site"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenValidation:SigningKey"])),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            ClaimsPrincipal principal;

            try
            {
                principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            }
            catch (Exception)
            {

                return null;
            }


            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                return null;

            return principal;
        }



        // ..................................................................................Get Access Token From Header.....................................................................
        private string GetAccessTokenFromHeader()
        {
            StringValues value;
            Request.Headers.TryGetValue("Authorization", out value);

            if (value.Count == 0) return null;

            Match result = Regex.Match(value, @"(?:Bearer\s)(.+)");
            return result.Groups[1].Value;
        }





        // ..................................................................................Get Host.....................................................................
        private string GetHost()
        {
            if (env.IsDevelopment())
            {
                return "http://localhost:4200";
            }

            return HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value;
        }
    }
}