using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrainEase.Core.Application.Abstrations.Services;
using TrainEase.Core.Application.Services;
using TrainEase.Core.Application.Services.Journey;
using TrainEase.Core.Application.Services.Stations;

namespace TrainEase.Core.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddAutoMapper(typeof(AssemblyInfo));
            services.AddScoped<IServiceManager, ServiceManager>();
            services.AddScoped<IJourneyService, JourneyService>();

            services.AddScoped<Func<IJourneyService>>(provider =>
            {
                return () => provider.GetRequiredService<IJourneyService>();
            });

            services.AddScoped<IStationService, StationService>();
            services.AddScoped<Func<IStationService>>(provider =>
            {
                return () => provider.GetRequiredService<IStationService>();
            });

            return services;
        }
    }
}
