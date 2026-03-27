using Microsoft.AspNetCore.Authorization;

namespace projectDemo.config
{
    public class PermissionRequirement:IAuthorizationRequirement
    {
        public string PermissonsName { get; }

        public PermissionRequirement(string permission)
        {
            PermissonsName = permission;
        }
    }
}
