using Microsoft.EntityFrameworkCore;
using TrainEase.Core.Application.Abstrations.Model.Booking;
using TrainEase.Core.Application.Abstrations.Services;
using TrainEase.Core.Domain.Contracts;
using TrainEase.Core.Domain.Entities;
using TrainEase.Core.Domain.Entities.Enums;

namespace TrainEase.Core.Application.Services.Booking
{
    public class BookingService : IBookingService
    {
        private readonly IAppDbContext _context;

        public BookingService(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateBookingAsync(CreateBookingDto dto, string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var schedule = await _context.Schedules.FirstOrDefaultAsync(s => s.Id == dto.ScheduleId);
                if (schedule == null) throw new Exception("Schedule not found");

                // 1. إنشاء الحجز الرئيسي
                var newBooking = new TrainEase.Core.Domain.Entities.Booking
                {
                    ScheduleId = dto.ScheduleId,
                    UserId = userId,
                    BookingDate = DateTime.Now,
                    TotalAmount = schedule.Price * dto.Passengers.Count,
                    Status = BookingStatus.Confirmed
                };
                _context.Bookings.Add(newBooking);
                await _context.SaveChangesAsync();

                foreach (var pDto in dto.Passengers)
                {
                    // 2. إنشاء الراكب
                    var passenger = new Passenger
                    {
                        Name = pDto.FullName,
                        Email = pDto.Email ?? $"{pDto.NationalId}@trainease.com",
                        PhoneNumber = pDto.PhoneNumber,
                        Password = "DefaultPassword123!",
                        NationalId = pDto.NationalId,
                        Age = 25
                    };
                    _context.Passengers.Add(passenger);
                    await _context.SaveChangesAsync(); // عشان ناخد Passenger.Id

                    // 3. إنشاء التذكرة (تربط المقعد بالحجز والمستخدم)
                    var ticket = new Ticket
                    {
                        BookingId = newBooking.Id,
                        SeatId = pDto.SeatId,
                        UserId = userId,
                        Price = schedule.Price
                    };
                    _context.Tickets.Add(ticket);
                    await _context.SaveChangesAsync(); // عشان ناخد Ticket.Id

                    // 4. الخطوة المفقودة: ربط الراكب بالتذكرة في جدول الـ BookTicket
                    var bookTicket = new BookTicket
                    {
                        PassengerId = passenger.Id,
                        TicketId = ticket.Id,
                        Price = schedule.Price,
                        CreatedOn = DateTime.Now
                    };
                    _context.BookTickets.Add(bookTicket);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return newBooking.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // هيطلع لك السبب الحقيقي لو لسه فيه مشكلة في الـ SQL
                var message = ex.InnerException?.Message ?? ex.Message;
                throw new Exception($"Database Error: {message}");
            }
        }

        public async Task<IEnumerable<SeatDto>> GetSeatsByScheduleAsync(int scheduleId)
        {
            // 1. جلب الرحلة مع القطار والعربات والمقاعد (الهيكل الفيزيائي)
            var schedule = await _context.Schedules
                .Include(s => s.Train)
                    .ThenInclude(t => t.Carriages)
                        .ThenInclude(c => c.Seats)
                .FirstOrDefaultAsync(s => s.Id == scheduleId);

            if (schedule == null) return Enumerable.Empty<SeatDto>();

            // 2. جلب الـ SeatIds المحجوزة لهذه الرحلة من جدول التذاكر
            // نذهب للحجوزات التابعة لهذه الرحلة، ثم نجلب التذاكر المرتبطة بها
            var bookedSeatIds = await _context.Tickets
                .Where(t => t.Booking.ScheduleId == scheduleId && t.Booking.Status != BookingStatus.Cancelled)
                .Select(t => t.SeatId)
                .ToListAsync();

            // 3. بناء الـ DTOs والمقارنة بالـ ID
            var seatDtos = schedule.Train.Carriages
                .SelectMany(c => c.Seats)
                .Select(s => new SeatDto
                {
                    Id = s.Id,
                    SeatNumber = s.SeatNumber,
                    // المقعد محجوز إذا كان الـ Id الخاص به موجوداً في قائمة التذاكر لهذه الرحلة
                    IsBooked = bookedSeatIds.Contains(s.Id)
                })
                .ToList();

            return seatDtos;
        }
    }
}
