using Company.G03.DAL.Entities;
using Company.G03.PL.Dtos;
using Company.G03.PL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Company.G03.PL.Controllers
    {
    [Authorize]
    public class RoleController : Controller
        {

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
            {
            _roleManager = roleManager;
            _userManager = userManager;
            }

        public IActionResult Index(string? Search)
            {
            IEnumerable<ReturnRoleDto> Roles;
            if (string.IsNullOrEmpty(Search))
                {
                Roles = _roleManager.Roles.Select(u => new ReturnRoleDto()
                    {
                    Id = u.Id,
                    Name = u.Name
                    });
                }
            else
                {
                Roles = _roleManager.Roles.Select(u => new ReturnRoleDto()
                    {
                    Id = u.Id,
                    Name = u.Name,
                    }).Where(u => u.Name.ToLower().Contains(Search.ToLower()));
                }

            return View(Roles);
            }

        #region Create

        [HttpGet]
        public IActionResult Create()
            {
            return View();
            }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReturnRoleDto model)
            {
            if (ModelState.IsValid) //server side validation
                {

                var role = await _roleManager.FindByNameAsync(model.Name);
                if (role is null)
                    {
                    role = new IdentityRole()
                        {
                        Name = model.Name
                        };
                    var result = await _roleManager.CreateAsync(role);
                    if (result.Succeeded)
                        {
                        return RedirectToAction(nameof(Index));
                        }

                    }
                ModelState.AddModelError("", "Role already exists");


                }
            return View(model);
            }

        #endregion

        #region Details

        [HttpGet]
        public async Task<IActionResult> Details(string? id, string viewName = "Details")
            {
            if (id is null)
                {
                return NotFound();
                }

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                {
                return NotFound();
                }
            var userDto = new ReturnRoleDto()
                {
                Id = role.Id,
                Name = role.Name
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
                return NotFound(new { StatusCode = 404, message = $"Employee with ID: {id} is not found" });
                }
            var role = _roleManager.FindByIdAsync(id).Result;
            if (role == null)
                {
                return NotFound();
                }
            var roleDto = new ReturnRoleDto()
                {
                Id = role.Id,
                Name = role.Name
                };
            return View(roleDto);

            }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] string id, ReturnRoleDto model)
            {

            if (ModelState.IsValid)
                {
                if (id != model.Id)
                    {
                    return BadRequest("Invalid Operation");
                    }
                var role = await _roleManager.FindByIdAsync(id);
                if (role is null)
                    {
                    return BadRequest("Invalid Operation");
                    }

                var Result = await _roleManager.FindByNameAsync(model.Name);
                if (Result is null)
                    {
                    role.Id = model.Id;
                    role.Name = model.Name;

                    var result = await _roleManager.UpdateAsync(role);

                    if (result.Succeeded)
                        {
                        return RedirectToAction(nameof(Index));
                        }
                    }
                ModelState.AddModelError("", "Invalid Operation");

                }
            return View(model);
            }
        #endregion

        #region Delete

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string? id)
            {
            if (id is null) return BadRequest();

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound();


            await _roleManager.DeleteAsync(role);

            return RedirectToAction(nameof(Index));
            }

        #endregion

        #region AddOrRemoveUser

        [HttpGet]
        public async Task<IActionResult> AddOrRemoveUsers(string roleId)
            {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role is null)
                {
                return NotFound();
                }
            ViewData["RoleId"] = role.Id;
            var usersInRole = new List<UsersInRoleViewModel>();
            var users = await _userManager.Users.ToListAsync();

            foreach (var user in users)
                {
                var userInRole = new UsersInRoleViewModel()
                    {
                    UserId = user.Id,
                    UserName = user.UserName
                    };
                if (await _userManager.IsInRoleAsync(user, role.Name))
                    {
                    userInRole.IsSelected = true;
                    }
                else
                    {
                    userInRole.IsSelected = false;
                    }
                usersInRole.Add(userInRole);
                }

            return View(usersInRole);
            }


        [HttpPost]
        public async Task<IActionResult> AddOrRemoveUsers(string roleId, List<UsersInRoleViewModel> users)
            {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role is null)
                {
                return NotFound();
                }

            if (ModelState.IsValid)
                {

                foreach (var user in users)
                    {

                    var appUser = await _userManager.FindByIdAsync(user.UserId);
                    if (appUser is not null)
                        {
                        if (user.IsSelected && !(await _userManager.IsInRoleAsync(appUser, role.Name)))
                            {
                            await _userManager.AddToRoleAsync(appUser, role.Name);
                            }

                        else if (!user.IsSelected && await _userManager.IsInRoleAsync(appUser, role.Name))
                            {
                            await _userManager.RemoveFromRoleAsync(appUser, role.Name);
                            }

                        }
                    }
                return RedirectToAction(nameof(Edit), new { id = role.Id });
                }

            return View(users);
            }
        #endregion

        }
    }
