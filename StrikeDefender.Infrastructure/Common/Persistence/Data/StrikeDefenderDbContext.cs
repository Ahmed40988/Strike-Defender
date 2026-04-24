using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StrikeDefender.Application.Common.Interfaces;
using StrikeDefender.Domain.Attacks;
using StrikeDefender.Domain.IntelligenceDataset;
using StrikeDefender.Domain.Payments;
using StrikeDefender.Domain.Plans;
using StrikeDefender.Domain.Rules;
using StrikeDefender.Domain.Subscriptions;
using StrikeDefender.Domain.Users;
using System.Data;
using System.Reflection;
namespace StrikeDefender.Infrastructure.Common.Persistence.Data
{
    public class StrikeDefenderDbContext:IdentityDbContext<AppUser>,IUnitOfWork
    {

        public StrikeDefenderDbContext(DbContextOptions<StrikeDefenderDbContext> options) : base(options)
        { }

        public async Task CommitChangesAsync()
        {
            await SaveChangesAsync();
        }
    public DbSet<Attack> Attacks => Set<Attack>();
    public DbSet<WafRule> wafRules => Set<WafRule>();
    public DbSet<Plan> Planes => Set<Plan>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<SecurityIntelligenceEntry> securityIntelligenceEntries => Set<SecurityIntelligenceEntry>();
    public DbSet<AttackResult> AttackResults => Set<AttackResult>();
    public DbSet<SuccessfulAttack> SuccessfulAttacks => Set<SuccessfulAttack>();
    public DbSet<PaymentTransaction>  paymentTransactions => Set<PaymentTransaction>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Important for Identity

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            var cascadeFKs = modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}


