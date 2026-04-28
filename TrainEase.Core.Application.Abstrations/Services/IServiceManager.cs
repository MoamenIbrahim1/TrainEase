using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainEase.Core.Application.Abstrations.Services
{
    public interface IServiceManager
    {
    public IAuthService AuthService { get; }
    public IJourneyService JourneyService { get; }
    public IStationService StationService { get; }
    public IAdminService AdminService { get; }
    public IDashboardService DashboardService { get; }
    public ITrainService TrainService { get; }
    public IBookingService BookingService { get; }
    public IPaymentService PaymentService { get; }
    public ITrainDelayService TrainDelayService { get; }
    public IOfferService OfferService { get; }

    }
}
