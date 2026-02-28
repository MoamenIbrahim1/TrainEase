using Microsoft.AspNetCore.Identity;
using System.Numerics;
using TrainEase.Core.Application.Abstrations.Services;
using TrainEase.Core.Application.NewFolder.Auth;
using TrainEase.Core.Application.Services.Auth;
using TrainEase.Core.Domain.Entities;
using TrainEase.Infrastructure.Presistance._Data;

namespace TrainEase.APIs.Extensions
{
    public static class IdentityExtensions
    {
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection("jwtSettings"));
          services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>();

            
            services.AddScoped(typeof(IAuthService), typeof(AuthService));

         services.AddScoped(typeof(Func<IAuthService>), (servicesProvider) =>
         {
             return () => servicesProvider.GetRequiredService<IAuthService>();
         } );



            return services;
        }
    }
}
