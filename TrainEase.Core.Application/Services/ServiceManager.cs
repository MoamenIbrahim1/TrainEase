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
        private readonly Lazy<IJourneyService> _journeyService;
        private readonly Lazy<IStationService> _stationService;

        public ServiceManager(IUnitOfWork unitOfWork, Func<IAuthService> authServiceFactory, Func<IJourneyService> journeyServiceFactory,Func<IStationService> stationServiceFactory)
        {
            _authService = new Lazy<IAuthService>(authServiceFactory, LazyThreadSafetyMode.ExecutionAndPublication);
            _journeyService = new Lazy<IJourneyService>( journeyServiceFactory, LazyThreadSafetyMode.ExecutionAndPublication);
            _stationService = new Lazy<IStationService>(stationServiceFactory, LazyThreadSafetyMode.ExecutionAndPublication);
        }
        public IAuthService AuthService => _authService.Value;
        public IJourneyService JourneyService => _journeyService.Value;
        public IStationService StationService => _stationService.Value;
    }
}