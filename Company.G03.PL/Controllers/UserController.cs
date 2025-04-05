using Company.G03.DAL.Entities;
using Company.G03.PL.Dtos;
using Company.G03.PL.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Company.G03.PL.Controllers
    {
    [Authorize]
    public class UserController : Controller
        {
        private readonly UserManager<AppUser> _userManager;

        public UserController(UserManager<AppUser> userManager)
            {
            _userManager = userManager;
            }

        public IActionResult Index(string? Search)
            {
            IEnumerable<ReturnUserDto> users;
            if (string.IsNullOrEmpty(Search))
                {
                users = _userManager.Users.Select(u => new ReturnUserDto()
                    {
                    Id = u.Id,
                    UserName = u.UserName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Roles = _userManager.GetRolesAsync(u).Result
                    });
                }
            else
                {
                users = _userManager.Users.Select(u => new ReturnUserDto()
                    {
                    Id = u.Id,
                    UserName = u.UserName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Roles = _userManager.GetRolesAsync(u).Result
                    }).Where(u => u.FirstName.ToLower().Contains(Search.ToLower()));
                }

            return View(users);
            }


        #region Details

        [HttpGet]
        public async Task<IActionResult> Details(string? id, string viewName = "Details")
            {
            if (id is null)
                {
                return NotFound();
                }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                {
                return NotFound();
                }
            var userDto = new ReturnUserDto()
                {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Roles = _userManager.GetRolesAsync(user).Result
                };
            return View(viewName, userDto);
            }

        #endregion

        #region Update

        [HttpGet]
        public IActionResult Edit(string? id)
            {
            if (id is null)
                {
                return NotFound(new { StatusCode = 404, message = $"User with ID: {id} is not found" });
                }
            var user = _userManager.FindByIdAsync(id).Result;
            if (user == null)
                {
                return NotFound();
                }
            var userDto = new ReturnUserDto()
                {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Roles = _userManager.GetRolesAsync(user).Result
                };
            return View(userDto);

            }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] string id, ReturnUserDto model)
            {

            if (ModelState.IsValid)
                {
                if (id != model.Id)
                    {
                    return BadRequest("Invalid Operation");
                    }
                var user = await _userManager.FindByIdAsync(id);
                if (user is null)
                    {
                    return BadRequest("Invalid Operation");
                    }

                user.UserName = model.UserName;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                    {
                    return RedirectToAction(nameof(Index));
                    }

                }
            return View(model);
            }
        #endregion

        #region Delete

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string? id)
            {
            if (id is null)
                {
                return BadRequest();
                }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                {
                return NotFound();
                }

            await _userManager.DeleteAsync(user);

            return RedirectToAction(nameof(Index));
            }

        #endregion

        }
    }
