using Microsoft.AspNetCore.Authorization;

namespace SSHKeysManager.Auth.PolicyRequirements
{
    public class IsAdministratorOrOwnerRequirement : IAuthorizationRequirement
    {

    }
}
