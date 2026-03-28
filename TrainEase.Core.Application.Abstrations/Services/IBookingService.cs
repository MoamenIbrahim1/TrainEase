using TrainEase.Core.Application.Abstrations.Model.Booking;

namespace TrainEase.Core.Application.Abstrations.Services
{
    public interface IBookingService
    {
        Task<IEnumerable<SeatDto>> GetSeatsByScheduleAsync(int scheduleId);
        Task<BookingResultDto> CreateBookingAsync(CreateBookingDto dto, string userId);
    }
}
