using Microsoft.EntityFrameworkCore;
using StrikeDefender.Domain.Plans;
using StrikeDefender.Infrastructure.Common.Persistence.Data;
namespace StrikeDefender.Infrastructure.Common.Persistence.Seeding
{ 
    public static class PlanSeeder
    {

        public static async Task SeedAsync(StrikeDefenderDbContext context)
        {
            if (await context.Planes.AnyAsync())
                return;

            var free = Plan.Create(
                name: "Free",
                price: 0,
                maxAttacks: 10,
                maxRules: 10,
                durationInDays: 3650,
                maxRiskScoreAccess: 20).Value;

            var pro = Plan.Create(
                name: "Pro",
                price: 200,
                maxAttacks: 100,
                maxRules: 100,
                durationInDays: 30,
                maxRiskScoreAccess: 70).Value;

            var enterprise = Plan.Create(
                name: "Enterprise",
                price: 500,
                maxAttacks: 1000,
                maxRules: 1000,
                durationInDays: 30,
                maxRiskScoreAccess: 100).Value;

            await context.Planes.AddRangeAsync(free, pro, enterprise);
            await context.SaveChangesAsync();
        }
    }
}
