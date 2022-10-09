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
using static Website.Classes.Enums;
using System.Drawing.Drawing2D;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Website.Classes.Notifications;

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


        private string CreatePassword()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();
        }

        [HttpGet]
        [Route("ValidateEmail")]
        public async Task<ActionResult> ValidateEmail(string email)
        {
            Customer customer = await userManager.FindByEmailAsync(email);

            if (customer != null) return Ok(new { duplicateEmail = true });

            return Ok();
        }



        [HttpGet]
        [Route("CheckEmail")]
        public async Task<ActionResult> CheckEmail(string email)
        {
            if (await unitOfWork.Customers.Any(x => x.Email == email))
            {
                return Ok();
            }

            return Ok(new { noEmail = true });
        }



        [HttpGet]
        [Route("ValidatePassword")]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> ValidatePassword(string password)
        {
            Customer customer = await userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (customer != null && password != null)
            {

                if (await userManager.CheckPasswordAsync(customer, password))
                {
                    return Ok();
                }

                return Ok(new { incorrectPassword = true });
            }

            return BadRequest();
        }


        [HttpGet]
        [Route("ValidateDeleteAccountOneTimePassword")]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> ValidateDeleteAccountOneTimePassword(string oneTimePassword)
        {
            Customer customer = await userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (customer != null && oneTimePassword != null)
            {
                if (await GetOneTimePassword(OtpType.DeleteAccount, customer.Id, oneTimePassword) != null)
                {
                    return Ok();
                }

                return Ok(new { incorrectOneTimePassword = true });
            }

            return BadRequest();
        }





        [HttpGet]
        [Route("ValidateEmailChangeOneTimePassword")]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> ValidateEmailChangeOneTimePassword(string oneTimePassword)
        {
            Customer customer = await userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (customer != null && oneTimePassword != null)
            {
                if (await GetOneTimePassword(OtpType.EmailChange, customer.Id, oneTimePassword) != null)
                {
                    return Ok();
                }

                return Ok(new { incorrectOneTimePassword = true });
            }

            return BadRequest();
        }




        


        // ..................................................................................Sign Up.....................................................................
        [HttpPost]
        [Route("SignUp")]
        public async Task<ActionResult> SignUp(Account account)
        {
            Customer customer = account.CreateCustomer();

            // Add the new customer to the database
            IdentityResult result = await userManager.CreateAsync(customer, account.Password);


            if (result.Succeeded)
            {
                string oneTimePassword = await SetOneTimePassword(OtpType.ActivateAccount, customer.Id, async () => await userManager.GenerateEmailConfirmationTokenAsync(customer));

                // Send an email to activate the account
                //await SendAccountActivationEmail(customer, oneTimePassword);

                // Create the first list
                //await CreateList(customer);



                //Create the UserName notification
                NewNotification newNotification = new NewNotification
                {
                    Type = (int)NotificationType.UserName,
                    UserName = customer.FirstName + " " + customer.LastName
                };
                await unitOfWork.Notifications.CreateNotification(newNotification, customer.Id);


                // The new customer was successfully added to the database
                return Ok();
            }


            return BadRequest();
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
                string oneTimePassword = await SetOneTimePassword(OtpType.ActivateAccount, customer.Id, async () => await userManager.GenerateEmailConfirmationTokenAsync(customer));

                await SendAccountActivationEmail(customer, oneTimePassword);
            }

            return Ok();
        }






        private async Task SendAccountActivationEmail(Customer customer, string oneTimePassword)
        {
            await emailService.SendEmail(EmailType.AccountActivation, "Activate your Niche Shack account", new Recipient
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email
            }, new EmailProperties
            {
                Var1 = oneTimePassword
            });
        }


        // ..................................................................................Create List.....................................................................
        private async Task CreateList(Customer customer)
        {
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
        }



        // ..................................................................................Log In.....................................................................
        [HttpPost]
        [Route("LogIn")]
        public async Task<ActionResult> LogIn(LogIn logIn)
        {
            // Get the customer from the database based on the email address
            Customer customer = await userManager.FindByEmailAsync(logIn.Email);


            if (customer == null || await userManager.CheckPasswordAsync(customer, logIn.Password) == false)
            {
                return Ok(new { notEmailPasswordMatch = true });
            }


            if (!customer.EmailConfirmed)
            {
                return Ok(new { notActivated = true });
            }


            await SetLogIn(customer, logIn.IsPersistent);

            return Ok();
        }




        // ..................................................................................External Login.....................................................................
        [HttpPost]
        [Route("ExternalLogin")]
        public async Task<IActionResult> ExternalLogin(SocialUser socialUser)
        {
            Customer customer = await userManager.FindByEmailAsync(socialUser.Email);

            if (customer == null)
            {
                var account = new Account
                {
                    FirstName = socialUser.FirstName,
                    LastName = socialUser.LastName,
                    Email = socialUser.Email
                };

                customer = account.CreateCustomer();

                IdentityResult result = await userManager.CreateAsync(customer);

                if (result.Succeeded)
                {
                    customer.EmailConfirmed = true;

                    await unitOfWork.Save();

                    // Create the first list
                    await CreateList(customer);
                }
            }

            string provider = socialUser.Provider.Substring(0, 1) + socialUser.Provider.Substring(1).ToLower();

            ExternalLogin externalLogin = new ExternalLogin(provider, await userManager.HasPasswordAsync(customer));

            await SetLogIn(customer, true, externalLogin);

            return Ok();
        }



        // ..................................................................................Set LogIn.....................................................................
        private async Task SetLogIn(Customer customer, bool isPersistent, ExternalLogin externalLogin = null)
        {
            List<Claim> claims = GetClaims(customer, isPersistent, externalLogin);

            var tokenData = await GenerateTokenData(customer, claims);

            SetCookies(tokenData, customer, isPersistent, externalLogin);
        }









        // ..................................................................................Set Cookies.....................................................................
        private void SetCookies(TokenData tokenData, Customer customer, bool isPersistent, ExternalLogin externalLogin)
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
            Response.Cookies.Append("customer", GetCustomerData(customer, externalLogin), cookieOptions);
        }





        // ..................................................................................Get Customer Data.....................................................................
        private string GetCustomerData(Customer customer, ExternalLogin externalLogin)
        {
            return customer.FirstName + "," + customer.LastName + "," + customer.Email + "," + customer.Image + (externalLogin != null && externalLogin.Provider != null ? "," + externalLogin.Provider : "") + (externalLogin != null && externalLogin.HasPassword ? "," + externalLogin.HasPassword : "");
        }



        // ..................................................................................Get Claims.....................................................................
        private List<Claim> GetClaims(Customer customer, bool isPersistent, ExternalLogin externalLogin)
        {
            List<Claim> claims = new List<Claim>()
                {
                    new Claim("acc", "customer"),
                    new Claim(ClaimTypes.NameIdentifier, customer.Id),
                    new Claim(JwtRegisteredClaimNames.Iss, configuration["TokenValidation:Site"]),
                    new Claim(JwtRegisteredClaimNames.Aud, configuration["TokenValidation:Site"]),
                    new Claim(ClaimTypes.IsPersistent, isPersistent.ToString())
                };

            if (externalLogin != null)
            {
                claims.Add(new Claim("externalLoginProvider", externalLogin.Provider));
                claims.Add(new Claim("hasPassword", externalLogin.HasPassword.ToString()));
            }


            return claims;
        }



        private async Task<OneTimePassword> GetOneTimePassword(OtpType otpType, string customerId, string password = null)
        {
            OneTimePassword oneTimePassword;

            if (password != null)
            {
                oneTimePassword = await unitOfWork.OneTimePasswords.Get(x => x.Password == password && x.Type == (int)otpType && x.CustomerId == customerId);
            }
            else
            {
                oneTimePassword = await unitOfWork.OneTimePasswords.Get(x => x.Type == (int)otpType && x.CustomerId == customerId);
            }


            if (oneTimePassword == null) return null;

            // The one time password has expired
            if (DateTime.Compare(DateTime.Now, oneTimePassword.Expires) > 0)
            {
                unitOfWork.OneTimePasswords.Remove(oneTimePassword);
                await unitOfWork.Save();

                return null;
            }

            return oneTimePassword;
        }



        private async Task<string> GetOneTimePasswordToken(string password, OtpType otpType, string customerId)
        {
            OneTimePassword oneTimePassword = await GetOneTimePassword(otpType, customerId, password);

            if (oneTimePassword == null) return null;

            string token = oneTimePassword.Token;

            unitOfWork.OneTimePasswords.Remove(oneTimePassword);

            await unitOfWork.Save();

            return token;
        }


        private async Task<string> SetOneTimePassword(OtpType otpType, string customerId, Func<Task<string>> GenerateToken)
        {
            OneTimePassword oneTimePassword = await GetOneTimePassword(otpType, customerId);

            if (oneTimePassword == null)
            {
                string password = CreatePassword();

                oneTimePassword = new OneTimePassword
                {
                    CustomerId = customerId,
                    Password = password,
                    Type = (int)otpType,
                    Token = await GenerateToken(),
                    Expires = DateTime.Now.AddHours(Convert.ToInt32(configuration["OneTimePasswords:ExpiresInHours"]))
                };

                unitOfWork.OneTimePasswords.Add(oneTimePassword);
                await unitOfWork.Save();
            }

            return oneTimePassword.Password;
        }






        [HttpPost]
        [Route("ValidateActivateAccountOneTimePassword")]
        public async Task<ActionResult> ValidateActivateAccountOneTimePassword(EmailOTP emailOTP)
        {
            Customer customer = await userManager.FindByEmailAsync(emailOTP.Email);

            if (customer != null && emailOTP.OneTimePassword != null)
            {
                if (await GetOneTimePassword(OtpType.ActivateAccount, customer.Id, emailOTP.OneTimePassword) != null)
                {
                    return Ok();
                }

                return Ok(new { incorrectOneTimePassword = true });
            }

            return BadRequest();


        }




        // ..................................................................................Activate Account.....................................................................
        [HttpPost]
        [Route("ActivateAccount")]
        public async Task<ActionResult> ActivateAccount(ActivateAccount activateAccount)
        {
            if (activateAccount.Email != null && activateAccount.OneTimePassword != null)
            {
                Customer customer = await userManager.FindByEmailAsync(activateAccount.Email);

                if (customer != null)
                {
                    string token = await GetOneTimePasswordToken(activateAccount.OneTimePassword, OtpType.ActivateAccount, customer.Id);


                    if (token != null)
                    {
                        // Confirm the email
                        await userManager.ConfirmEmailAsync(customer, token);

                        await SetLogIn(customer, true);

                        await emailService.SendEmail(EmailType.WelcomeToNicheShack, "Welcome to Niche Shack", new Recipient
                        {
                            FirstName = customer.FirstName,
                            LastName = customer.LastName,
                            Email = customer.Email
                        });

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
                            },
                            Host = GetHost()
                        });
                    }


                    UpdateCustomerCookie(customer);


                    // Create the UserName notification
                    NewNotification newNotification = new NewNotification
                    {
                        Type = (int)NotificationType.UserName,
                        UserName = customer.FirstName + " " + customer.LastName
                    };
                    await unitOfWork.Notifications.CreateNotification(newNotification, customer.Id);


                    return Ok();
                }
            }


            return BadRequest();
        }





        // ..................................................................................Update Customer Cookie.....................................................................
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

            ExternalLogin externalLogin = new ExternalLogin(User.FindFirstValue("externalLoginProvider"), User.FindFirstValue("hasPassword") == "True");

            Response.Cookies.Append("customer", GetCustomerData(customer, externalLogin), cookieOptions);
        }




        // ..................................................................................Update Password.....................................................................
        [HttpPut]
        [Route("ChangePassword")]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> ChangePassword(UpdatedPassword updatedPassword)
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

            return BadRequest();
        }





        // ..................................................................................Change Profile Picture.....................................................................
        [HttpPost, DisableRequestSizeLimit]
        [Route("ChangeProfilePicture")]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> ChangeProfilePicture()
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

            // If the customer is found
            if (customer != null)
            {
                //Update the customer's profile picture
                customer.Image = imageName;

                // Update the database
                IdentityResult result = await userManager.UpdateAsync(customer);

                if (result.Succeeded)
                {
                    // Send a confirmation email that the customer image has been changed
                    //if (customer.EmailPrefProfilePicChange == true)
                    //{
                    //    await emailService.SendEmail(EmailType.ProfilePicChange, "Updated profile picture", new Recipient
                    //    {
                    //        FirstName = customer.FirstName,
                    //        LastName = customer.LastName,
                    //        Email = customer.Email
                    //    });
                    //}

                    UpdateCustomerCookie(customer);




                    // Create the UserName notification
                    NewNotification newNotification = new NewNotification
                    {
                        Type = (int)NotificationType.UserImage,
                        UserImage = customer.Image
                    };
                    await unitOfWork.Notifications.CreateNotification(newNotification, customer.Id);




                    return Ok();
                }
            }


            return BadRequest();
        }











        // ............................................................................Create Delete Account OTP.................................................................
        [HttpPost]
        [Route("CreateDeleteAccountOTP")]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> CreateDeleteAccountOTP()
        {
            string customerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Customer customer = await userManager.FindByIdAsync(customerId);


            string password = await SetOneTimePassword(OtpType.DeleteAccount, customerId, async () => await userManager.GenerateUserTokenAsync(customer, TokenOptions.DefaultProvider, "Delete Account"));


            await emailService.SendEmail(EmailType.DeleteAccountOneTimePassword, "Delete Account", new Recipient
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


            // If the customer is found...
            if (customer != null)
            {
                string token = await GetOneTimePasswordToken(otp.OneTimePassword, OtpType.DeleteAccount, customer.Id);

                if (await userManager.VerifyUserTokenAsync(customer, TokenOptions.DefaultProvider, "Delete Account", token) && await userManager.CheckPasswordAsync(customer, otp.Password))
                {
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

                    return Ok();
                }
            }
            return BadRequest();
        }




        [HttpGet]
        [Route("AddPassword")]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> AddPassword(string password)
        {
            // Get the customer from the database based on the customer id from the claims via the access token
            Customer customer = await userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (customer != null && password != null)
            {
                IdentityResult result = await userManager.AddPasswordAsync(customer, password);


                if (result.Succeeded)
                {
                    ExternalLogin externalLogin = new ExternalLogin(User.FindFirstValue("externalLoginProvider"), true);
                    await SetLogIn(customer, bool.Parse(User.FindFirstValue(ClaimTypes.IsPersistent)), externalLogin);
                    return Ok();
                }
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


                            ExternalLogin externalLogin = new ExternalLogin(principal.FindFirstValue("externalLoginProvider"), principal.FindFirstValue("hasPassword") == "True");

                            SetCookies(tokenData, customer, bool.Parse(principal.FindFirstValue(ClaimTypes.IsPersistent)), externalLogin);

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

                if (result != null) return Ok(new { duplicateEmail = true });

                string password = await SetOneTimePassword(OtpType.EmailChange, customerId, async () => await userManager.GenerateChangeEmailTokenAsync(customer, email));


                await emailService.SendEmail(EmailType.EmailOneTimePassword, "Change Email", new Recipient
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


            // If the customer is found...
            if (customer != null)
            {
                if (await userManager.CheckPasswordAsync(customer, otp.Password))
                {
                    string token = await GetOneTimePasswordToken(otp.OneTimePassword, OtpType.EmailChange, customer.Id);

                    if (token != null)
                    {
                        // Change the customer's email
                        IdentityResult result = await userManager.ChangeEmailAsync(customer, otp.Email, token);


                        if (result.Succeeded)
                        {
                            string previousEmail = customer.Email;

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

                            return Ok();
                        }
                    }
                }

            }

            return BadRequest();
        }








        // ..................................................................................Forgot Password.....................................................................
        [HttpGet]
        [Route("ForgotPassword")]
        public async Task<ActionResult> ForgotPassword(string email)
        {
            // Get the customer from the database based on the email address
            Customer customer = await userManager.FindByEmailAsync(email);

            if (customer != null)
            {
                string oneTimePassword = await SetOneTimePassword(OtpType.ResetPassword, customer.Id, async () => await userManager.GeneratePasswordResetTokenAsync(customer));

                await emailService.SendEmail(EmailType.ResetPassword, "Forgot Password", new Recipient
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email
                }, new EmailProperties
                {
                    Var1 = oneTimePassword
                });

                return Ok();
            }


            return BadRequest();
        }







        // ..................................................................................Validate Reset Password OTP.....................................................................
        [HttpPost]
        [Route("ValidateResetPasswordOTP")]
        public async Task<ActionResult> ValidateResetPasswordOTP(EmailOTP otp)
        {
            // Get the customer from the database based on the email address
            Customer customer = await userManager.FindByEmailAsync(otp.Email);


            if (customer != null && otp.OneTimePassword != null)
            {
                if (await GetOneTimePassword(OtpType.ResetPassword, customer.Id, otp.OneTimePassword) != null)
                {
                    return Ok();
                }

                return Ok(new { incorrectOneTimePassword = true });
            }

            return BadRequest();
        }



        // ..................................................................................Reset Password.....................................................................
        [HttpPost]
        [Route("ResetPassword")]
        public async Task<ActionResult> ResetPassword(ResetPassword resetPassword)
        {
            if (resetPassword.Email != null && resetPassword.Password != null)
            {
                Customer customer = await userManager.FindByEmailAsync(resetPassword.Email);

                string token = await GetOneTimePasswordToken(resetPassword.OneTimePassword, OtpType.ResetPassword, customer.Id);

                if (customer != null)
                {
                    var result = await userManager.ResetPasswordAsync(customer, token, resetPassword.Password);


                    if (result.Succeeded)
                    {
                        return Ok();
                    }
                }

            }

            return BadRequest();
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