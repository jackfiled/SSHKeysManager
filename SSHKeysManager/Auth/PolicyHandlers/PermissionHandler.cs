using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using SSHKeysManager.Auth.PolicyRequirements;
using SSHKeysManager.Common;

namespace SSHKeysManager.Auth.PolicyHandlers
{
    /// <summary>
    /// 处理用户权限要求
    /// </summary>
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
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
}
