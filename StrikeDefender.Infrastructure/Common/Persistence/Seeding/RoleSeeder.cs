using Microsoft.AspNetCore.Identity;

namespace StrikeDefender.Infrastructure.Common.Persistence.Seeding
{
    public class RoleSeeder
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleSeeder(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            string[] roles =
            [
                "Admin",
                "User",
                "CyberSpecialistAdmin"
            ];

            foreach (var role in roles)
            {
                var exists = await _roleManager.RoleExistsAsync(role);
                if (!exists)
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}