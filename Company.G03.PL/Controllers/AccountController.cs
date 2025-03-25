using Company.G03.DAL.Entities;
using Company.G03.PL.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Company.G03.PL.Controllers
    {
    public class AccountController : Controller
        {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
            {
            _userManager = userManager;
            _signInManager = signInManager;
            }

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
                            TermsAndConditions = model.TermsAndConditions
                            };

                        var result = await _userManager.CreateAsync(User, model.Password);

                        if (result.Succeeded)
                            {
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


        [HttpGet]
        public new async Task<IActionResult> SignOut()
            {
            await _signInManager.SignOutAsync();
            return RedirectToAction("SignIn");
            }

        }
    }
