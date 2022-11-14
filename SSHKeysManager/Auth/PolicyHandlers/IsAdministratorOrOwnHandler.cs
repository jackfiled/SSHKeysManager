using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using SSHKeysManager.Auth.PolicyRequirements;
using SSHKeysManager.Common;
using SSHKeysManager.Models;

namespace SSHKeysManager.Auth.PolicyHandlers
{
    /// <summary>
    /// 验证请求用户是否为管理员
    /// </summary>
    public class IsAdministratorHandler : AuthorizationHandler<IsAdministratorOrOwnerRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsAdministratorOrOwnerRequirement requirement)
        {
            var permissionString = context.User.FindFirstValue("Permission");

            if (permissionString == null)
            {
                return Task.CompletedTask;
            }

            if (int.TryParse(permissionString, out int permission))
            {
                if (permission >= (int)UserPermission.Administrator)
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// 验证请求用户是否为本人
    /// </summary>
    public class IsOwnerHandler : AuthorizationHandler<IsAdministratorOrOwnerRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsAdministratorOrOwnerRequirement requirement)
        {
            string idString = context.User.FindFirstValue("id");

            if (idString == null)
            {
                return Task.CompletedTask;
            }

            if (context.Resource is HttpContext httpContext)
            {
                string path = httpContext.Request.Path;

                if (path.Contains(idString))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
