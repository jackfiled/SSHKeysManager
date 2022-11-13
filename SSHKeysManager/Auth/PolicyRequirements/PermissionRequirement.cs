using Microsoft.AspNetCore.Authorization;
using SSHKeysManager.Common;

namespace SSHKeysManager.Auth.PolicyRequirements
{
    /// <summary>
    /// 授权中用户权限要求
    /// </summary>
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public UserPermission permission { get; }

        public PermissionRequirement(UserPermission permission)
        {
            this.permission = permission;
        }
    }
}
