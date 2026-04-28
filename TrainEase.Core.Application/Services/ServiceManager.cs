using TrainEase.Core.Application.Abstrations.Services;
using TrainEase.Core.Domain.Contracts;

namespace TrainEase.Core.Application.Services
{
    public class ServiceManager : IServiceManager
    {

        private readonly Lazy<IAuthService> _authService;
        private readonly Lazy<IJourneyService> _journeyService;
        private readonly Lazy<IStationService> _stationService;
        private readonly Lazy<IAdminService> _adminService;
        private readonly Lazy<ITrainService> _trainService;
        private readonly Lazy<IBookingService> _bookingService;
        private readonly Lazy<IPaymentService> _paymentService;
        private readonly Lazy<IDashboardService> _dashboardService;
        private readonly Lazy<ITrainDelayService> _trainDelayService;
        private readonly Lazy<IOfferService> _offerService;

        public ServiceManager(IUnitOfWork unitOfWork, Func<IAuthService> authServiceFactory, Func<IJourneyService> journeyServiceFactory, Func<IStationService> stationServiceFactory, Func<IAdminService> adminServiceFactory, Func<ITrainService> trainServiceFactory, Func<IBookingService> bookingServiceFactory,Func<IPaymentService> paymentServiceFactory, Func<IDashboardService> dashboardServiceFactory, Func<ITrainDelayService> trainDelayServiceFactory, Func<IOfferService> offerServiceFactory)
        {
            _authService = new Lazy<IAuthService>(authServiceFactory, LazyThreadSafetyMode.ExecutionAndPublication);
            _journeyService = new Lazy<IJourneyService>(journeyServiceFactory, LazyThreadSafetyMode.ExecutionAndPublication);
            _stationService = new Lazy<IStationService>(stationServiceFactory, LazyThreadSafetyMode.ExecutionAndPublication);
            _adminService = new Lazy<IAdminService>(adminServiceFactory, LazyThreadSafetyMode.ExecutionAndPublication);
            _trainService = new Lazy<ITrainService>(trainServiceFactory, LazyThreadSafetyMode.ExecutionAndPublication);
            _bookingService = new Lazy<IBookingService>(bookingServiceFactory, LazyThreadSafetyMode.ExecutionAndPublication);
            _paymentService = new Lazy<IPaymentService>(paymentServiceFactory, LazyThreadSafetyMode.ExecutionAndPublication);
            _dashboardService = new Lazy<IDashboardService>(dashboardServiceFactory, LazyThreadSafetyMode.ExecutionAndPublication);
            _trainDelayService = new Lazy<ITrainDelayService>(trainDelayServiceFactory, LazyThreadSafetyMode.ExecutionAndPublication);
            _offerService = new Lazy<IOfferService>(offerServiceFactory, LazyThreadSafetyMode.ExecutionAndPublication);
        }
        public IAuthService AuthService => _authService.Value;
        public IJourneyService JourneyService => _journeyService.Value;
        public IStationService StationService => _stationService.Value;
        public IAdminService AdminService => _adminService.Value;
        public ITrainService TrainService => _trainService.Value;
        public IBookingService BookingService => _bookingService.Value;
        public IPaymentService PaymentService => _paymentService.Value;
        public IDashboardService DashboardService => _dashboardService.Value;
        public ITrainDelayService TrainDelayService => _trainDelayService.Value;
        public IOfferService OfferService => _offerService.Value;
    }
}