using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StrikeDefender.Application.Common.Interfaces;
using StrikeDefender.Infrastructure.Users.Persistance;
using System.Reflection;
namespace StrikeDefender.Infrastructure.Common.Persistence.Data
{
    public class AppDbContext:IdentityDbContext<AppUser>,IUnitOfWork
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        public async Task CommitChangesAsync()
        {
            await SaveChangesAsync();
        }
      //  public DbSet<BaseCategory> BaseCategories => Set<BaseCategory>();

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
