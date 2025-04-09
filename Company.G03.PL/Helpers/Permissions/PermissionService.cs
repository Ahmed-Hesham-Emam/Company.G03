using Company.G03.BLL.Interfaces;
using Company.G03.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Company.G03.PL.Helpers.Permissions
    {
    public class PermissionService : IPermissionService
        {
        private readonly UserManager<AppUser> _userManager;
        private readonly IPermissionRepository _permissionRepository;

        public PermissionService(UserManager<AppUser> userManager, IPermissionRepository permissionRepository)
            {
            _userManager = userManager;
            _permissionRepository = permissionRepository;
            }

        public async Task<bool> UserHasPermission(ClaimsPrincipal userPrincipal, string permissionName)
            {
            var user = await _userManager.GetUserAsync(userPrincipal);
            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
                {

                var roleEntity = await _permissionRepository.GetRoleByNameAsync(role);
                if (roleEntity == null)
                    {
                    continue;
                    }

                var permissions = await _permissionRepository.GetPermissionNamesByRoleAsync(roleEntity.Id);

                if (permissions.Assigned.Contains(permissionName))
                    {
                    return true;
                    }
                }

            return false;
            }
        }
    }
