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
        private readonly UserContext userContext;

        public IsOwnerHandler(UserContext userContext)
        {
            // 通过令牌中的接收者为用户的电子邮件地址
            // 在数据库中拿到用户的id
            this.userContext = userContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsAdministratorOrOwnerRequirement requirement)
        {
            string emailAddress = context.User.FindFirstValue("aud");

            if (emailAddress == null)
            {
                return Task.CompletedTask;
            }

            var user = userContext.Users.Single(u => u.EmailAddress== emailAddress);

            // 这里其实是反向验证
            // 判断请求的路径中是否有请求用户的id
            if (context.Resource is HttpContext httpContext)
            {
                string requestPath = httpContext.Request.Path;

                if (requestPath.Contains(user.Id.ToString()))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
