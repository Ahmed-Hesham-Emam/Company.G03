using Company.G03.DAL.Entities;
using Company.G03.PL.Dtos;
using Company.G03.PL.Helpers.Email;
using Company.G03.PL.Helpers.SMS;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Company.G03.PL.Controllers
    {
    public class AccountController : Controller
        {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly ITwilioService _twilioService;


        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService, ITwilioService twilioService)
            {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _twilioService = twilioService;
            }

        #region SignUp

        [HttpGet]
        public IActionResult SignUp()
            {
            return View();
            }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpDto model)
            {
            if (ModelState.IsValid)
                {
                var User = await _userManager.FindByNameAsync(model.UserName);
                if (User is null)
                    {

                    User = await _userManager.FindByEmailAsync(model.Email);
                    if (User is null)
                        {

                        User = new AppUser()
                            {
                            UserName = model.UserName,
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Email = model.Email,
                            PhoneNumber = model.PhoneNumber,
                            TermsAndConditions = model.TermsAndConditions
                            };

                        var result = await _userManager.CreateAsync(User, model.Password);

                        if (result.Succeeded)
                            {
                            await _userManager.AddToRoleAsync(User, "User");
                            return RedirectToAction("SignIn");
                            }

                        foreach (var error in result.Errors)
                            {
                            ModelState.AddModelError("", error.Description);
                            }

                        }

                    }

                ModelState.AddModelError("", "Invalid Register Data!");

                }

            return View(model);
            }

        #endregion

        #region SignIn

        [HttpGet]
        public IActionResult SignIn()
            {
            return View();
            }


        [HttpPost]
        public async Task<IActionResult> SignIn(SignInDto model)
            {
            if (ModelState.IsValid)
                {
                var User = await _userManager.FindByEmailAsync(model.Email);

                if (User is not null)
                    {
                    if (await _userManager.IsLockedOutAsync(User))
                        {
                        var Lockout = await _userManager.GetLockoutEnabledAsync(User);
                        ModelState.AddModelError("", $"Your account is locked out for {Lockout}, please try again later.");
                        return View(model);

                        }
                    var flag = await _userManager.CheckPasswordAsync(User, model.Password);

                    if (flag)
                        {
                        var resualt = await _signInManager.PasswordSignInAsync(User, model.Password, model.RememberMe, false);
                        if (resualt.Succeeded)
                            {
                            return RedirectToAction("Index", "Home");
                            }
                        }
                    }

                ModelState.AddModelError("", "Invalid Login Data!");

                }

            return View(model);
            }

        #endregion

        #region SignOut

        [HttpGet]
        public new async Task<IActionResult> SignOut()
            {
            await _signInManager.SignOutAsync();
            return RedirectToAction("SignIn");
            }

        #endregion

        #region PasswordReset
        [HttpGet]
        public IActionResult ForgotPassword()
            {
            return View();
            }

        #region Email

        [HttpPost]
        public async Task<IActionResult> SendResetPasswordUrl(ForgotPasswordDto model)
            {
            if (ModelState.IsValid)
                {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user is not null)
                    {
                    // Token Generation
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    // URL Creation
                    var url = Url.Action("ResetPassword", "Account", new { email = model.Email, token }, Request.Scheme);

                    // Email Creation
                    var email = new Email()
                        {
                        EmailAddress = model.Email,
                        Subject = "Reset Password",
                        Body = url
                        };

                    //var flag = EmailSettings.SendEmail(email);
                    _emailService.SendEmail(email);

                    return RedirectToAction("CheckInbox");

                    }
                ModelState.AddModelError("", "Process Failed SUCCESSFULLY !");
                }

            return View("ForgotPassword", model);
            }

        #endregion

        #region SMS

        [HttpPost]
        public async Task<IActionResult> SendResetPasswordUrlSms(ForgotPasswordDto model)
            {
            if (ModelState.IsValid)
                {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user is not null)
                    {
                    // Token Generation
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    // URL Creation
                    var url = Url.Action("ResetPassword", "Account", new { email = model.Email, token }, Request.Scheme);

                    // SMS Creation
                    var sms = new Sms()
                        {
                        To = user.PhoneNumber,
                        body = url
                        };

                    _twilioService.SendMessage(sms);

                    return RedirectToAction("CheckSms");

                    }
                ModelState.AddModelError("", "Process Failed SUCCESSFULLY !");
                }

            return View("ForgotPassword", model);
            }

        #endregion

        [HttpGet]
        public IActionResult CheckInbox()
            {
            return View();
            }

        [HttpGet]
        public IActionResult CheckSms()
            {
            return View();
            }

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
            {
            TempData["Email"] = email;
            TempData["Token"] = token;
            return View();
            }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
            {

            if (ModelState.IsValid)
                {
                var email = TempData["Email"] as string;
                var token = TempData["Token"] as string;

                if (email is null || token is null)
                    {
                    return BadRequest("WHO ARE YOU?! (╯°□°）╯︵ ┻━┻");
                    }

                var user = await _userManager.FindByEmailAsync(email);

                if (user is not null)
                    {
                    var res = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                    if (res.Succeeded)
                        {
                        return RedirectToAction("SignIn");
                        }
                    }
                ModelState.AddModelError("", "WHO ARE YOU?! (╯°□°）╯︵ ┻━┻");

                }
            return View();
            }

        #endregion

        #region AccessDenied

        public IActionResult AccessDenied()
            {
            return View();
            }

        #endregion

        #region Google
        public IActionResult GoogleLogin()
            {
            var prop = new AuthenticationProperties()
                {
                RedirectUri = Url.Action("GoogleResponse")
                };
            return Challenge(prop, GoogleDefaults.AuthenticationScheme);
            }

        public async Task<IActionResult> GoogleResponse()
            {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(
                claim => new
                    {
                    claim.Type,
                    claim.Value,
                    claim.Issuer,
                    claim.OriginalIssuer,
                    }
                );
            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var FirstName = result.Principal.FindFirst(ClaimTypes.GivenName)?.Value;
            var LastName = result.Principal.FindFirst(ClaimTypes.Surname)?.Value;
            var phoneNumber = result.Principal.FindFirst(ClaimTypes.MobilePhone)?.Value;
            //var UserName = result.Principal.FindFirst(ClaimTypes.Name)?.Value;

            var user = await _userManager.FindByEmailAsync(email);

            bool isNewUser = false;

            if (user == null)
                {
                user = new AppUser
                    {
                    UserName = Guid.NewGuid().ToString(),
                    Email = email,
                    FirstName = FirstName,
                    LastName = LastName,
                    PhoneNumber = phoneNumber,
                    TermsAndConditions = true,
                    };
                var newResualt = await _userManager.CreateAsync(user);
                if (!newResualt.Succeeded)
                    {
                    ModelState.AddModelError("", "Invalid Register Data!");
                    return View("SignUp");
                    }
                //await _userManager.CreateAsync(user);
                isNewUser = true;
                }

            await _signInManager.SignInAsync(user, isPersistent: false);
            if (isNewUser)
                {
                return RedirectToAction("ChooseUsername", new { userId = user.Id });
                }

            return RedirectToAction("Index", "Home");
            }

        #endregion

        #region Facebook

        public IActionResult FacebookLogin()
            {
            var prop = new AuthenticationProperties()
                {
                RedirectUri = Url.Action("FacebookResponse")
                };
            return Challenge(prop, FacebookDefaults.AuthenticationScheme);
            }

        public async Task<IActionResult> FacebookResponse()
            {
            var result = await HttpContext.AuthenticateAsync(FacebookDefaults.AuthenticationScheme);
            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(
                claim => new
                    {
                    claim.Type,
                    claim.Value,
                    claim.Issuer,
                    claim.OriginalIssuer,
                    }
                );
            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var FirstName = result.Principal.FindFirst(ClaimTypes.GivenName)?.Value;
            var LastName = result.Principal.FindFirst(ClaimTypes.Surname)?.Value;
            var phoneNumber = result.Principal.FindFirst(ClaimTypes.MobilePhone)?.Value;
            //var UserName = result.Principal.FindFirst(ClaimTypes.Name)?.Value;

            var user = await _userManager.FindByEmailAsync(email);

            bool isNewUser = false;

            if (user == null)
                {
                user = new AppUser
                    {
                    UserName = Guid.NewGuid().ToString(),
                    Email = email,
                    FirstName = FirstName,
                    LastName = LastName,
                    PhoneNumber = phoneNumber,
                    TermsAndConditions = true
                    };
                var newResualt = await _userManager.CreateAsync(user);
                if (!newResualt.Succeeded)
                    {
                    ModelState.AddModelError("", "Invalid Register Data!");
                    return View("SignUp");
                    }
                //await _userManager.CreateAsync(user);
                isNewUser = true;
                }

            await _signInManager.SignInAsync(user, isPersistent: false);


            if (isNewUser)
                {
                return RedirectToAction("ChooseUsername", new { userId = user.Id });
                }

            return RedirectToAction("Index", "Home");
            }

        #endregion

        #region Create Username and password after etxternal login for the first time

        [HttpGet]
        public async Task<IActionResult> ChooseUsername(string userId)
            {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            return View(new ChooseUsernameDto { UserId = userId });
            }


        [HttpPost]
        public async Task<IActionResult> ChooseUsername(ChooseUsernameDto model)
            {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

            user.UserName = model.UserName;


            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                {

                if (!await _userManager.IsInRoleAsync(user, "User"))
                    {
                    await _userManager.AddToRoleAsync(user, "User");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    }

                await _signInManager.RefreshSignInAsync(user);
                return RedirectToAction("Index", "Home");
                }
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
            }

        #endregion

        }
    }
