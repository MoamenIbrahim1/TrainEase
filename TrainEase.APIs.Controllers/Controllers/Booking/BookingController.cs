using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrainEase.Core.Application.Abstrations.Model.Booking;
using TrainEase.Core.Application.Abstrations.Services;
using TrainEase.Core.Domain.Entities;

namespace TrainEase.APIs.Controllers.Controllers.Booking
{

    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }


        [HttpGet("schedules/{scheduleId}/seats")]
        public async Task<IActionResult> GetSeats(int scheduleId)
        {
            var seats = await _bookingService.GetSeatsByScheduleAsync(scheduleId);
            return Ok(seats);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking(CreateBookingDto dto)
        {
            try
            {

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("يجب تسجيل الدخول أولاً لإتمام الحجز");
                }

                var bookingId = await _bookingService.CreateBookingAsync(dto, userId);
                return Ok(new { Message = "Booking Created Successfully", BookingId = bookingId });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
