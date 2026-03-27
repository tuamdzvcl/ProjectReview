using Microsoft.AspNetCore.Authorization;

namespace projectDemo.config
{
    public class PermissionAttribute : AuthorizeAttribute
    {
        public PermissionAttribute(string permission)
        {
            Policy = permission;
        }
    }
    }
