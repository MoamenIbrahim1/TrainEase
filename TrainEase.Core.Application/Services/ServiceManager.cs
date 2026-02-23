using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Application.Abstrations.Services;
using TrainEase.Core.Domain.Contracts;

namespace TrainEase.Core.Application.Services
{
    public class ServiceManager : IServiceManager
    {
    
        private readonly Lazy<IAuthService> _authService;

        public ServiceManager(IUnitOfWork unitOfWork, Func<IAuthService> authServiceFactory)
        {
            _authService = new Lazy<IAuthService>(authServiceFactory, LazyThreadSafetyMode.ExecutionAndPublication);
        }
        public IAuthService AuthService => _authService.Value;
    }
}
