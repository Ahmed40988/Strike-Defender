
using Microsoft.Extensions.DependencyInjection;
using StrikeDefender.Application.Common.Authorization;
using StrikeDefender.Application.Common.Behaviors;

namespace StrikeDefender.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(options =>
            {
                options.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection));
                options.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjection));


       

            return services;
        }


    }
}
