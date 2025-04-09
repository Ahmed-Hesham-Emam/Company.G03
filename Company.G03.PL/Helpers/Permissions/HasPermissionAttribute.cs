using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Company.G03.PL.Helpers.Permissions
    {
    public class HasPermissionAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
        {

        private readonly string _permission;

        public HasPermissionAttribute(string permission)
            {
            _permission = permission;
            }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
            {
            var user = context.HttpContext.User;

            if (user != null)
                if (!user.Identity.IsAuthenticated)
                    {
                    context.Result = new ForbidResult();
                    return;
                    }

            var permissionService = context.HttpContext.RequestServices.GetRequiredService<IPermissionService>();
            var hasPermission = await permissionService.UserHasPermission(user, _permission);

            if (!hasPermission)
                {
                context.Result = new ForbidResult();
                }
            }

        }
    }
