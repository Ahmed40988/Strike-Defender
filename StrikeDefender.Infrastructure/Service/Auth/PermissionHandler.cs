using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using StrikeDefender.Application.Common.Authorization;
using StrikeDefender.Domain.Users;

namespace Web.Infrastructure.Service.Auth
{
    public class PermissionHandler
        : AuthorizationHandler<PermissionRequirement>
    {
        private readonly UserManager<AppUser> _userManager;

        public PermissionHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var user = await _userManager.GetUserAsync(context.User);
            if (user is null)
                return;

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                if (RolePermissions.Map.TryGetValue(role, out var permissions))
                {
                    if (permissions.Contains(requirement.Permission))
                    {
                        context.Succeed(requirement);
                        return;
                    }
                }
            }
        }
    }
    }
