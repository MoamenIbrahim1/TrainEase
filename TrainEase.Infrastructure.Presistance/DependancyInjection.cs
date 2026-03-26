using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Application.Abstrations.Services;
using TrainEase.Core.Application.Services;
using TrainEase.Core.Application.Services.Journey;
using TrainEase.Core.Domain.Contracts;
using TrainEase.Core.Domain.Entities;
using TrainEase.Infrastructure.Presistance._Data;
using TrainEase.Infrastructure.Presistance._Identity;
using TrainEase.Infrastructure.Presistance.Repostories;
using TrainEase.Infrastructure.Presistance.UnitOfWork;
namespace TrainEase.Infrastructure.Presistance
{
    public static class InfrastructureDependencies
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
          
            #region IdentityContext
            services.AddDbContext<AppDbContext>(options =>
              options.UseLazyLoadingProxies().UseSqlServer(configuration.GetConnectionString("IdentityContext")));
            #endregion
            services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
            services.AddScoped<IUnitOfWork, AppUnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            return services;
        }
    }
}