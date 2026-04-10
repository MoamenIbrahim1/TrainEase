using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrainEase.APIs.Controllers.Controllers.Base;
using TrainEase.Core.Application.Abstrations.Model.Booking;
using TrainEase.Core.Application.Abstrations.Services;

namespace TrainEase.APIs.Controllers.Controllers.Booking
{
    [Authorize]
    public class BookingsController(IServiceManager serviceManager) : BaseApiController
    {
        [HttpGet("journeys/{journeyId}/seats")]
        public async Task<IActionResult> GetSeats(int journeyId)
        {
            var result = await serviceManager.BookingService
                .GetSeatsByScheduleAsync(journeyId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await serviceManager.BookingService
                .CreateBookingAsync(dto, userId);
            return Ok(result);
        }

        [HttpGet("AllBookingsByUser")]
        public async Task<IActionResult> GetMyBookings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await serviceManager.BookingService
                .GetBookingsByUserAsync(userId);
            return Ok(result);
        }
        [HttpGet("AllBookings")]
        public async Task<IActionResult> GetAllBookings()
        {
            var result = await serviceManager.BookingService
                .GetAllBookingsAsync();
            return Ok(result);
        }
    }
}