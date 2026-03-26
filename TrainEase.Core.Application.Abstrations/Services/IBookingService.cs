using TrainEase.Core.Application.Abstrations.Model.Booking;

namespace TrainEase.Core.Application.Abstrations.Services
{
    public interface IBookingService
    {
        // أضفنا string userId هنا
        Task<int> CreateBookingAsync(CreateBookingDto dto, string userId);
        Task<IEnumerable<SeatDto>> GetSeatsByScheduleAsync(int scheduleId);
    }
}
