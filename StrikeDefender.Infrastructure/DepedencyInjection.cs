using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StrikeDefender.Application.Common.Interfaces;
using StrikeDefender.Domain.Plans;
using StrikeDefender.Domain.Subscriptions;
using StrikeDefender.Domain.Users;
using StrikeDefender.Infrastructure.Common.Persistence.Data;
using StrikeDefender.Infrastructure.Common.Persistence.Seeding;
using StrikeDefender.Infrastructure.ExternalServices.AI.Configurations;
using StrikeDefender.Infrastructure.ExternalServices.AI.Helpers;
using StrikeDefender.Infrastructure.ExternalServices.AI.Providers;
using StrikeDefender.Infrastructure.ExternalServices.AI.Services;
using StrikeDefender.Infrastructure.Plans.Persistance;
using StrikeDefender.Infrastructure.Service.FuzzzySearch;
using StrikeDefender.Infrastructure.Services.Files;
using StrikeDefender.Infrastructure.Subscriptions.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Infrastructure.Service.Auth;
using Web.Infrastructure.Users.Persistence;

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
                .AddAIServices(configuration);
        }

        public static IServiceCollection AddPersistence(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<StrikeDefenderDbContext>());
            services.AddScoped<RoleSeeder>();
            services.AddScoped<IFuzzySearchRepository, FuzzySearchRepository>();
            services.AddScoped<IFileHelperService, FileHelper>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISubscriptionAccessService, SubscriptionRepository>();
            services.AddScoped<IPlanRepository, PlanRepository>();
            services.AddScoped<IGenericRepository<Plan>, PlanRepository>();
            services.AddScoped<IGenericRepository<Subscription>, SubscriptionRepository>();

            return services;
        }
        private static IServiceCollection AddDatabaseConfig(this IServiceCollection services, IConfiguration configuration)
        {
        //var connectionString = configuration.GetConnectionString("DefaultConnection")
                 var connectionString = configuration.GetConnectionString("localhostConnection")
                ??
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<StrikeDefenderDbContext>(options =>
                options.UseSqlServer(connectionString));

            return services;
        }

        public static IServiceCollection AddAIServices(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<GeminiOptions>(
                config.GetSection("Gemini"));

            services.AddSingleton<AiRateLimiter>();
            services.AddSingleton<AiUsageTracker>();

            services.AddHttpClient<IAiProvider, GeminiProvider>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(60);
            });

            services.AddScoped<IAiEngineService, AiEngineService>();
            services.AddScoped<IAiProvider, GeminiProvider>();

            return services;
        }

        private static IServiceCollection AddIdentityConfig(this IServiceCollection services)
        {
            services.AddIdentityCore<AppUser>()
       .AddRoles<IdentityRole>()
       .AddEntityFrameworkStores<StrikeDefenderDbContext>()
       .AddDefaultTokenProviders();

            return services;
        }

    }
}
