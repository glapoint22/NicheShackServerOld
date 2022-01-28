using System;
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
using static Website.Classes.Enums;
using System.Drawing.Drawing2D;

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
                    Name = customer.FirstName + "'s List",
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


            return Ok(new { failure = true });
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
                return Ok(new { notActivated = true });
            }

            // If the customer is in the database and the password is valid, create claims for the access token
            if (customer != null && await userManager.CheckPasswordAsync(customer, signIn.Password) && customer.Active)
            {
                List<Claim> claims = GetClaims(customer, signIn.IsPersistent);

                var tokenData = await GenerateTokenData(customer, claims);

                SetCookies(tokenData, customer, signIn.IsPersistent);

                return Ok();
            }

            return Ok(new { noMatch = true });
        }





        private void SetCookies(TokenData tokenData, Customer customer, bool isPersistent)
        {
            CookieOptions cookieOptions = new CookieOptions();

            if (isPersistent)
            {
                cookieOptions = new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(Convert.ToInt32(configuration["TokenValidation:RefreshExpiresInDays"]))
                };
            }

            Response.Cookies.Append("access", tokenData.AccessToken, cookieOptions);
            Response.Cookies.Append("refresh", tokenData.RefreshToken, cookieOptions);
            Response.Cookies.Append("customer", GetCustomerData(customer), cookieOptions);
        }



        private string GetCustomerData(Customer customer)
        {
            return customer.FirstName + "," + customer.LastName + "," + customer.Email + "," + customer.Image;
        }



        // ..................................................................................Get Claims.....................................................................
        private List<Claim> GetClaims(Customer customer, bool isPersistent)
        {
            List<Claim> claims = new List<Claim>()
                {
                    new Claim("acc", "customer"),
                    new Claim(ClaimTypes.NameIdentifier, customer.Id),
                    new Claim(JwtRegisteredClaimNames.Iss, configuration["TokenValidation:Site"]),
                    new Claim(JwtRegisteredClaimNames.Aud, configuration["TokenValidation:Site"]),
                    new Claim(ClaimTypes.IsPersistent, isPersistent.ToString())
                };


            return claims;
        }




        // ..................................................................................Activate Account.....................................................................
        [HttpPost]
        [Route("ActivateAccount")]
        public async Task<ActionResult> ActivateAccount(ActivateAccount activateAccount)
        {
            if (activateAccount.Email != null && activateAccount.Token != null)
            {
                Customer customer = await userManager.FindByEmailAsync(activateAccount.Email);

                if (customer != null)
                {
                    var result = await userManager.ConfirmEmailAsync(customer, activateAccount.Token);

                    if (result.Succeeded)
                    {
                        List<Claim> claims = GetClaims(customer, true);

                        var tokenData = await GenerateTokenData(customer, claims);

                        SetCookies(tokenData, customer, true);

                        return Ok();
                    }
                }
            }


            return BadRequest();
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
                string originalFirstName = customer.FirstName;
                string originalLastName = customer.LastName;

                customer.FirstName = updatedCustomerName.FirstName;
                customer.LastName = updatedCustomerName.LastName;

                // Update the name in the database
                IdentityResult result = await userManager.UpdateAsync(customer);

                if (result.Succeeded)
                {
                    // Send a confirmation email that the customer name has been changed
                    if (customer.EmailPrefNameChange == true)
                    {
                        await emailService.SendEmail(EmailType.NameChange, "Name change confirmation", new Recipient
                        {
                            FirstName = customer.FirstName,
                            LastName = customer.LastName,
                            Email = customer.Email
                        }, new EmailProperties
                        {
                            Person = new Person
                            {
                                FirstName = originalFirstName,
                                LastName = originalLastName
                            }
                        });
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


            Response.Cookies.Append("customer", GetCustomerData(customer), cookieOptions);
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
                        await emailService.SendEmail(EmailType.PasswordChange, "Password change confirmation", new Recipient
                        {
                            FirstName = customer.FirstName,
                            LastName = customer.LastName,
                            Email = customer.Email
                        });
                    }


                    return Ok();
                }
            }

            return Ok(true);
        }





        // ..................................................................................Change Profile Picture.....................................................................
        [HttpPost, DisableRequestSizeLimit]
        [Route("ChangeProfilePicture")]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> NewImage()
        {
            string wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string imagesFolder = Path.Combine(wwwroot, "images");


            //Get the form data
            IFormFile imageFile = Request.Form.Files["newImage"];
            Request.Form.TryGetValue("percentTop", out StringValues percentTop);
            Request.Form.TryGetValue("percentLeft", out StringValues percentLeft);
            Request.Form.TryGetValue("percentRight", out StringValues percentRight);
            Request.Form.TryGetValue("percentBottom", out StringValues percentBottom);
            Request.Form.TryGetValue("currentImage", out StringValues currentImage);


            double left;
            double right;
            double top;
            double bottom;

            using (var image = System.Drawing.Image.FromStream(imageFile.OpenReadStream()))
            {
                left = Convert.ToDouble(percentLeft) * image.Width;
                right = Convert.ToDouble(percentRight) * image.Width;
                top = Convert.ToDouble(percentTop) * image.Height;
                bottom = Convert.ToDouble(percentBottom) * image.Height;
            }




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


            int size = (int)(right - left);

            //Crop
            Bitmap croppedBitmap = new Bitmap(size, size);
            for (int i = 0; i < size; i++)
            {
                for (int y = 0; y < size; y++)
                {
                    Color pxlclr = tempBitmap.GetPixel((int)left + i, (int)top + y);
                    croppedBitmap.SetPixel(i, y, pxlclr);
                }
            }






            //Scale
            Bitmap scaledBitmap = new Bitmap(70, 70);
            Graphics graph = Graphics.FromImage(scaledBitmap);
            graph.InterpolationMode = InterpolationMode.High;
            graph.DrawImage(croppedBitmap, 0, 0, 70, 70);



            //Create the new image
            string imageName = Guid.NewGuid().ToString("N") + ".png";
            string newImage = Path.Combine(imagesFolder, imageName);
            scaledBitmap.Save(newImage, ImageFormat.Png);





            //If the customer currently has an image assigned to their profile
            if (!String.IsNullOrEmpty(currentImage))
            {
                // Delete that customer's current image
                System.IO.File.Delete(imagesFolder + "\\" + currentImage);
            }






            // Get the customer from the database based on the customer id from the claims via the access token
            Customer customer = await userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // If the customer is found, update his/her name
            if (customer != null)
            {
                //Update the customer's profile picture
                customer.Image = imageName;

                // Update the database
                IdentityResult result = await userManager.UpdateAsync(customer);

                if (result.Succeeded)
                {
                    // Send a confirmation email that the customer name has been changed
                    if (customer.EmailPrefProfilePicChange == true)
                    {
                        await emailService.SendEmail(EmailType.ProfilePicChange, "Updated profile picture", new Recipient
                        {
                            FirstName = customer.FirstName,
                            LastName = customer.LastName,
                            Email = customer.Email
                        });
                    }

                    UpdateCustomerCookie(customer);

                    return Ok();
                }
            }


            return BadRequest();
        }















        //private static System.Drawing.Image resizeImage(System.Drawing.Image imgToResize, Size size)
        //{
        //    //Get the image current width  
        //    int sourceWidth = imgToResize.Width;
        //    //Get the image current height  
        //    int sourceHeight = imgToResize.Height;
        //    float nPercent = 0;
        //    float nPercentW = 0;
        //    float nPercentH = 0;
        //    //Calulate  width with new desired size  
        //    nPercentW = ((float)size.Width / (float)sourceWidth);
        //    //Calculate height with new desired size  
        //    nPercentH = ((float)size.Height / (float)sourceHeight);
        //    if (nPercentH < nPercentW)
        //        nPercent = nPercentH;
        //    else
        //        nPercent = nPercentW;
        //    //New Width  
        //    int destWidth = (int)(sourceWidth * nPercent);
        //    //New Height  
        //    int destHeight = (int)(sourceHeight * nPercent);
        //    Bitmap b = new Bitmap(destWidth, destHeight);
        //    Graphics g = Graphics.FromImage((System.Drawing.Image)b);
        //    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //    // Draw image with new width and height  
        //    g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
        //    g.Dispose();
        //    return (System.Drawing.Image)b;
        //}

























        // ............................................................................Create Delete Account OTP.................................................................
        [HttpPost]
        [Route("CreateDeleteAccountOTP")]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> CreateDeleteAccountOTP()
        {
            string customerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Customer customer = await userManager.FindByIdAsync(customerId);
            string password = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();

            OneTimePassword otp = new OneTimePassword
            {
                CustomerId = customerId,
                Password = password,
                Type = (int)OtpType.AccountDeletion
            };

            unitOfWork.OneTimePasswords.Add(otp);
            await unitOfWork.Save();

            await emailService.SendEmail(EmailType.DeleteAccountOneTimePassword, "One-Time Password", new Recipient
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email
            }, new EmailProperties
            {
                Var1 = password
            });
            return Ok();
        }



        // ..................................................................................Delete Account.....................................................................
        [HttpPut]
        [Route("DeleteAccount")]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> DeleteAccount(OTP otp)
        {
            // Get the customer from the database based on the customer id from the claims via the access token
            Customer customer = await userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // Grab all the one-time passwords for the customer who is deleting their account
            // (typically there should only be ONE one-time password record, but it is possible that there could be more than one)
            IEnumerable<OneTimePassword> oneTimePasswords = await unitOfWork.OneTimePasswords.GetCollection(x => x.CustomerId == customer.Id && x.Type == (int)OtpType.AccountDeletion);

            // Check to see if the one-time password the customer entered on the form matches any of the one-time passwords in the list
            OneTimePassword oneTimePassword = oneTimePasswords.FirstOrDefault(x => x.Password == otp.OneTimePassword);


            // If the customer is found...
            if (customer != null)
            {
                // If either the password or the one-time password doesn't pass
                if (!await userManager.CheckPasswordAsync(customer, otp.Password) || oneTimePassword == null)
                {
                    // Fail
                    return Ok(new { failure = true });
                }


                // Send a confirmation email that the customer account has been deleted
                await emailService.SendEmail(EmailType.DeleteAccount, "Delete account confirmation", new Recipient
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email
                });

                // Grab all the lists from this customer
                IEnumerable<List> lists = await unitOfWork.Collaborators.GetCollection(x => x.CustomerId == customer.Id && x.IsOwner, x => x.List);

                // Remove the lists
                unitOfWork.Lists.RemoveRange(lists);

                // Remove the customer
                unitOfWork.Customers.Remove(customer);


                await unitOfWork.Save();


                return Ok(new { failure = false });
            }
            return BadRequest();
        }




















        // ..................................................................................Refresh.....................................................................
        [HttpGet]
        [Route("Refresh")]
        public async Task<ActionResult> Refresh()
        {
            string accessToken = GetAccessTokenFromHeader();
            string refreshTokenCookie = Request.Cookies["refresh"];

            if (accessToken != null)
            {
                ClaimsPrincipal principal = GetPrincipalFromToken(accessToken);


                if (principal != null && refreshTokenCookie != null)
                {
                    string customerId = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

                    if (customerId != null)
                    {
                        RefreshToken refreshToken = await unitOfWork.RefreshTokens.Get(x => x.Id == refreshTokenCookie && x.CustomerId == customerId);

                        if (refreshToken != null && DateTime.Compare(DateTime.UtcNow, refreshToken.Expiration) < 0)
                        {
                            Customer customer = await userManager.FindByIdAsync(customerId);

                            // Generate a new token and refresh token
                            TokenData tokenData = await GenerateTokenData(customer, principal.Claims);


                            SetCookies(tokenData, customer, bool.Parse(principal.FindFirstValue(ClaimTypes.IsPersistent)));

                            return Ok(new
                            {
                                value = tokenData.RefreshToken
                            });
                        }
                    }
                }
            }

            return Ok();
        }



        [HttpDelete]
        [Route("Refresh")]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> DeleteRefreshToken(string newRefreshToken)
        {
            Customer customer = await userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (customer != null)
            {
                // Get all refresh tokens from this customer except the new refresh token
                IEnumerable<RefreshToken> tokens = await unitOfWork.RefreshTokens.GetCollection(x => x.CustomerId == customer.Id && x.Id != HttpUtility.UrlDecode(newRefreshToken));

                // Delete the tokens
                if (tokens != null && tokens.Count() > 0)
                {
                    foreach (RefreshToken refreshToken in tokens)
                    {
                        // Remove the refresh token from the database
                        unitOfWork.RefreshTokens.Remove(refreshToken);
                    }

                    await unitOfWork.Save();
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

                return Ok();
            }


            return Ok(true);
        }









        // ..............................................................................Create Change Email OTP.....................................................................
        [HttpGet]
        [Route("CreateChangeEmailOTP")]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> CreateChangeEmailOTP(string email)
        {
            string customerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Customer customer = await userManager.FindByIdAsync(customerId);


            if (customer != null)
            {
                var result = await userManager.FindByEmailAsync(email);

                if (result != null) return Ok(true);


                string password = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();

                OneTimePassword otp = new OneTimePassword
                {
                    CustomerId = customerId,
                    Password = password,
                    Type = (int)OtpType.EmailChange
                };

                unitOfWork.OneTimePasswords.Add(otp);
                await unitOfWork.Save();


                await emailService.SendEmail(EmailType.EmailOneTimePassword, "One-Time Password", new Recipient
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = email
                }, new EmailProperties
                {
                    Var1 = password
                });
                return Ok();
            }
            return BadRequest();
        }





        // ..................................................................................Change Email.....................................................................
        [HttpPut]
        [Route("ChangeEmail")]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> ChangeEmail(EmailOTP otp)
        {
            // Get the customer from the database based on the customer id from the claims via the access token
            Customer customer = await userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // Grab all the one-time passwords for the customer who is changing their email
            // (typically there should only be ONE one-time password record, but it is possible that there could be more than one)
            IEnumerable<OneTimePassword> oneTimePasswords = await unitOfWork.OneTimePasswords.GetCollection(x => x.CustomerId == customer.Id && x.Type == (int)OtpType.EmailChange);

            // Check to see if the one-time password the customer entered on the form matches any of the one-time passwords in the list
            OneTimePassword oneTimePassword = oneTimePasswords.FirstOrDefault(x => x.Password == otp.OneTimePassword);


            // If the customer is found...
            if (customer != null)
            {
                // If either the password or the one-time password doesn't pass
                if (!await userManager.CheckPasswordAsync(customer, otp.Password) || oneTimePassword == null)
                {
                    // Fail
                    return Ok(new { failure = true });
                }


                string previousEmail = customer.Email;


                // Change the customer's email
                customer.Email = otp.Email;
                customer.NormalizedEmail = otp.Email.ToUpper();
                unitOfWork.Customers.Update(customer);

                // Remove all one-time passwords
                unitOfWork.OneTimePasswords.RemoveRange(oneTimePasswords);
                await unitOfWork.Save();


                UpdateCustomerCookie(customer);


                // Send a confirmation email that the customer email has been changed
                if (customer.EmailPrefEmailChange == true)
                {
                    await emailService.SendEmail(EmailType.EmailChange, "Email change confirmation", new Recipient
                    {
                        FirstName = customer.FirstName,
                        LastName = customer.LastName,
                        Email = otp.Email
                    }, new EmailProperties
                    {
                        Var1 = previousEmail,
                        Var2 = otp.Email
                    });
                }

                return Ok(new { failure = false });
            }
            return BadRequest();
        }





        // ..................................................................................Send Account Activation Email.....................................................................
        private async Task SendAccountActivationEmail(Customer customer)
        {
            string token = await userManager.GenerateEmailConfirmationTokenAsync(customer);
            string host = GetHost();

            await emailService.SendEmail(EmailType.AccountActivation, "Activate your new Niche Shack account", new Recipient
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email
            }, new EmailProperties
            {
                Link = host + "/activate-account?email=" + HttpUtility.UrlEncode(customer.Email) + "&token=" + HttpUtility.UrlEncode(token)
            });
        }






        // ..................................................................................Send Reset Password Email.....................................................................
        private async Task SendResetPasswordEmail(Customer customer)
        {
            string token = await userManager.GeneratePasswordResetTokenAsync(customer);
            string host = GetHost();

            await emailService.SendEmail(EmailType.ResetPassword, "Reset Password", new Recipient
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email
            }, new EmailProperties
            {
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