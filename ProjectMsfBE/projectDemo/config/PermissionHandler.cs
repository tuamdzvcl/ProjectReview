using Microsoft.AspNetCore.Authorization;

namespace projectDemo.config
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var permissions = context.User.Claims
            .Where(c => c.Type == "permission")
            .Select(c => c.Value);

            if (permissions.Contains(requirement.PermissonsName))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
    }

