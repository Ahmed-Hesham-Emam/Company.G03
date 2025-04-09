using System.Security.Claims;

namespace Company.G03.PL.Helpers.Permissions
    {
    public interface IPermissionService
        {
        Task<bool> UserHasPermission(ClaimsPrincipal userPrincipal, string permissionName);

        }
    }
