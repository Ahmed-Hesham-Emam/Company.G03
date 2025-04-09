using Company.G03.BLL.Interfaces;
using Company.G03.BLL.Repositories;
using Company.G03.DAL.Entities;
using Company.G03.PL.Dtos;
using Company.G03.PL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Threading.Tasks;

namespace Company.G03.PL.Controllers
    {
    [Authorize]
    public class RoleController : Controller
        {

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IPermissionRepository _permissionManager;

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, IPermissionRepository permissionManager)
            {
            _roleManager = roleManager;
            _userManager = userManager;
            _permissionManager = permissionManager;
            }
        #region Index
        public async Task<IActionResult> Index(string? Search)
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
                Roles = _roleManager.Roles
                    .Where(u => u.Name.ToLower().Contains(Search.ToLower()))
                    .Select(u => new ReturnRoleDto()
                        {
                        Id = u.Id,
                        Name = u.Name
                        });

                }

            return View(Roles);
            }

        #endregion

        #region Search

        public async Task<IActionResult> Search(string Search)
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

            return PartialView("RolePartialView/_RoleTablePartialView", Roles);
            }

        #endregion

        #region Create

        [HttpGet]
        public async Task<IActionResult> Create()
            {
            var allPermissions = await _permissionManager.GetAllPermissionsAsync(); // Get list of all permissions
            var model = new ReturnRoleDto
                {
                UnassignedPermissions = allPermissions.Select(p => p.Name).ToList(),
                AssignedPermissions = new List<string>()
                };
            return View(model);
            }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReturnRoleDto model)
            {
            if (ModelState.IsValid)
                {
                var roleExists = await _roleManager.FindByNameAsync(model.Name);
                if (roleExists is null)
                    {
                    var newRole = new IdentityRole { Name = model.Name };
                    var result = await _roleManager.CreateAsync(newRole);

                    if (result.Succeeded)
                        {
                        // Assign permissions
                        await _permissionManager.UpdateRolePermissionsAsync(newRole.Id, model.AssignedPermissions);
                        TempData["Message"] = "Role created successfully!";
                        return RedirectToAction(nameof(Index));
                        }
                    }

                ModelState.AddModelError("", "Role already exists.");
                }

            // Refill unassigned permissions in case of model error
            var allPermissions = await _permissionManager.GetAllPermissionsAsync();
            model.UnassignedPermissions = allPermissions.Select(p => p.Name).Except(model.AssignedPermissions).ToList();

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
        public async Task<IActionResult> Edit(string? id)
            {
            if (id is null)
                {
                return NotFound(new { StatusCode = 404, message = $"Role with ID: {id} is not found" });
                }

            var role = await _roleManager.FindByIdAsync(id);
            if (role is null)
                {
                return NotFound();
                }

            var (assignedPermissions, unassignedPermissions) = await _permissionManager.GetPermissionNamesByRoleAsync(role.Id);

            var roleDto = new ReturnRoleDto
                {
                Id = role.Id,
                Name = role.Name,
                AssignedPermissions = assignedPermissions,
                UnassignedPermissions = unassignedPermissions
                };

            return View(roleDto);

            }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] string id, ReturnRoleDto model)
            {

            if (!ModelState.IsValid)
                {
                return View(model);
                }

            if (id != model.Id)
                {
                return BadRequest("Invalid Operation");
                }

            var role = await _roleManager.FindByIdAsync(id);
            if (role is null)
                {
                return BadRequest("Role not found.");
                }

            // Check for duplicate role name
            var existingRole = await _roleManager.FindByNameAsync(model.Name);
            if (existingRole != null && existingRole.Id != id)
                {
                ModelState.AddModelError("", "A role with this name already exists.");
                return View(model);
                }

            role.Name = model.Name;

            // Update permissions
            await _permissionManager.UpdateRolePermissionsAsync(role.Id, model.AssignedPermissions);

            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
                {
                TempData["Message"] = "Role updated successfully.";
                return RedirectToAction(nameof(Index));
                }

            foreach (var error in result.Errors)
                {
                ModelState.AddModelError("", error.Description);
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
