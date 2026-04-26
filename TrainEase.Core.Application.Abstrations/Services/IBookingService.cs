using TrainEase.Core.Application.Abstrations.Model.Booking;
using TrainEase.Core.Application.Abstrations.Model.Journey;

namespace TrainEase.Core.Application.Abstrations.Services
{
    public interface IBookingService
    {
        Task<JourneySeatsDto> GetSeatsByScheduleAsync(int scheduleId);
        Task<BookingResultDto> CreateBookingAsync(CreateBookingDto dto, string userId);
        Task<IEnumerable<BookingResultDto>> GetBookingsByUserAsync(string userId);
        Task<IEnumerable<BookingResultDto>> GetAllBookingsAsync();
        Task<CancelBookingResultDto> CancelBookingAsync(int bookingId, string userId);
        Task<BookingResultDto> GetBookingAsync(int id);
    }
}
