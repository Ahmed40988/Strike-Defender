
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using StrikeDefender.Application.Common.Interfaces;
using StrikeDefender.Infrastructure.Service.Communication.Email;
using System.Reflection;
using System.Text;

namespace StrikeDefender.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
        {

            services
                .AddControllersConfig()
                .AddCorsConfig()
                .AddSwaggerWithAuth()
                    .AddJwtAuthConfig(configuration)
                    .AddAuthorizationConfig()
                .AddEmailConfig(configuration)
                .AddAppServices();

            return services;
        }


        private static IServiceCollection AddControllersConfig(this IServiceCollection services)
        {
            services.AddControllers();
            return services;
        }


        private static IServiceCollection AddCorsConfig(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            return services;
        }

        private static IServiceCollection AddSwaggerWithAuth(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Strike Defender",
                    Version = "v1",
                    Description = "Authentication & Authorization APIs"
                });

                // 🔹 JWT Auth
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Enter JWT token with Bearer prefix (Example: 'Bearer eyJhbGciOi...')",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
            });

            return services;
        }
        private static IServiceCollection AddEmailConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));

            return services;
        }

        private static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddScoped<IEmailSender, EmailService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddMemoryCache();

            return services;
        }

        private static IServiceCollection AddJwtAuthConfig(
    this IServiceCollection services,
    IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidAudience = configuration["JWT:Audience"],

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            configuration["JWT:Key"]
                                ?? throw new InvalidOperationException("JWT Key is missing")
                        ))
                };
            });

            return services;
        }

        private static IServiceCollection AddAuthorizationConfig(
    this IServiceCollection services)
        {
            services.AddAuthorization();
            return services;
        }

    }
}
