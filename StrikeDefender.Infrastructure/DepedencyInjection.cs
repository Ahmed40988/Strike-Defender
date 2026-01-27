using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddPersistence()
                .AddDatabaseConfig(configuration)
                .AddIdentityConfig()
                .AddAuthorizationConfig();
        }

        public static IServiceCollection AddPersistence(this IServiceCollection services)
        {
            //services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<AppDbContext>());
            //services.AddScoped<IFuzzySearchRepository, FuzzySearchRepository>();
            //services.AddScoped<TokenService, TokenService>();
            //services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }
        private static IServiceCollection AddDatabaseConfig(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            //services.AddDbContext<AppDbContext>(options =>
            //    options.UseSqlServer(connectionString));

            return services;
        }

        private static IServiceCollection AddIdentityConfig(this IServiceCollection services)
        {
            //services.AddIdentity<AppUser, IdentityRole>()
            //    .AddEntityFrameworkStores<AppDbContext>()
            //    .AddDefaultTokenProviders();

            return services;
        }

        private static IServiceCollection AddAuthorizationConfig(this IServiceCollection services)
        {
            services.AddAuthorization();
            return services;
        }
    }
}
