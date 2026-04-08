using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrainEase.Core.Application.Abstrations.Model.Payment;
using TrainEase.Core.Application.Abstrations.Services;
using TrainEase.Core.Application.Services;
using TrainEase.Core.Application.Services.Admin;
using TrainEase.Core.Application.Services.Booking;
using TrainEase.Core.Application.Services.Journey;
using TrainEase.Core.Application.Services.Payment;
using TrainEase.Core.Application.Services.Stations;
using TrainEase.Core.Application.Services.Trains;

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

            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<Func<IAdminService>>(provider =>
            {
                return () => provider.GetRequiredService<IAdminService>();
            });

            services.AddScoped<ITrainService, TrainService>();
            services.AddScoped<Func<ITrainService>>(provider =>
                {
                    return () => provider.GetRequiredService<ITrainService>();
                });

            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<Func<IBookingService>>(provider =>
            {
                return () => provider.GetRequiredService<IBookingService>();
            });

            services.AddScoped<IPaymentService,PaymentService>();
            services.AddScoped<Func<IPaymentService>>(provider =>
            {
                return () => provider.GetRequiredService<IPaymentService>();
            });

            services.Configure<PaymobSettings>(configuration.GetSection("Paymob"));
            services.AddHttpClient<IPaymentService, PaymentService>();

            return services;
        }
    }
}
