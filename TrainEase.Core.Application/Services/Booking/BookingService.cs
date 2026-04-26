using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TrainEase.Core.Application.Abstrations.Model.Booking;
using TrainEase.Core.Application.Abstrations.Model.Carriage;
using TrainEase.Core.Application.Abstrations.Model.Journey;
using TrainEase.Core.Application.Abstrations.Model.Seat;
using TrainEase.Core.Application.Abstrations.Services;
using TrainEase.Core.Application.Exceptions;
using TrainEase.Core.Application.Services.Email;
using TrainEase.Core.Domain.Contracts;
using TrainEase.Core.Domain.Entities;
using TrainEase.Core.Domain.Entities.Enums;

namespace TrainEase.Core.Application.Services.Booking
{
    public class BookingService(IUnitOfWork unitOfWork, UserManager<User> userManager, IEmailService emailService) : IBookingService
    {
        private const int PendingBookingExpirationMinutes = 15;


        public async Task<JourneySeatsDto> GetSeatsByScheduleAsync(int scheduleId)
        {
            await ExpireOldPendingBookingsAsync(scheduleId: scheduleId);

            var scheduleRepo = unitOfWork.GetRepository<Schedule, int>();

            var schedule = await scheduleRepo
                .GetQueryable(s => s.Id == scheduleId && !s.IsDeleted)
                .Include(s => s.Train)
                    .ThenInclude(t => t.Carriages.Where(c => !c.IsDeleted))
                        .ThenInclude(c => c.Seats.Where(s => !s.IsDeleted))
                .FirstOrDefaultAsync();

            if (schedule is null)
                throw new NotFoundException("Schedule", scheduleId);

            var cutoff = DateTime.UtcNow.AddMinutes(-PendingBookingExpirationMinutes);

            var ticketRepo = unitOfWork.GetRepository<Ticket, int>();
            var bookedSeatIds = await ticketRepo
                .GetQueryable(t =>
                    t.Booking.ScheduleId == scheduleId &&
                    !t.Booking.IsDeleted &&
                    (
                        t.Booking.Status == BookingStatus.Confirmed ||
                        (t.Booking.Status == BookingStatus.Pending && t.Booking.BookingDate >= cutoff)
                    ))
                .Select(t => t.SeatId)
                .ToListAsync();

            var carriages = schedule.Train.Carriages
                .Select(c => new CarriageSeatsDto
                {
                    CarriageId = c.Id,
                    CarriageNumber = c.CarriageNumber!,
                    ClassType = c.ClassType!,
                    SeatsCount = c.Seats.Count,
                    PricePerSeat = c.PricePerSeat,
                    Seats = c.Seats.Select(s => new SeatInfoDto
                    {
                        SeatId = s.Id,
                        SeatNumber = s.SeatNumber!,
                        IsBooked = bookedSeatIds.Contains(s.Id)
                    }).ToList()
                }).ToList();

            return new JourneySeatsDto
            {
                TrainId = schedule.Train.Id,
                TrainNumber = schedule.Train.TrainNumber!,
                TrainName = schedule.Train.TrainName!,
                Carriages = carriages
            };
        }
        public async Task<BookingResultDto> CreateBookingAsync(CreateBookingDto dto, string userId)
        {
            if (dto is null)
                throw new BadRequestException("Request body is required.");

            await ExpireOldPendingBookingsAsync(scheduleId: dto.ScheduleId);

            var user = await userManager.FindByIdAsync(userId);

            if (user is null)
                throw new UnAuthorizedException("User not found.");

            if (string.IsNullOrWhiteSpace(user.FullName) ||
                string.IsNullOrWhiteSpace(user.NationalId) ||
                string.IsNullOrWhiteSpace(user.Email) ||
                string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                throw new BadRequestException("Your account data is incomplete. Please complete your profile first.");
            }

            var additionalPassengers = dto.AdditionalPassengers ?? new List<PassengerBookingDto>();

            var allPassengers = new List<PassengerBookingDto>
            {
                new PassengerBookingDto
                {
                    FullName = user.FullName,
                    NationalId = user.NationalId,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber!,
                    SeatId = dto.UserSeatId
                }
            };

            allPassengers.AddRange(additionalPassengers);

            var requestedSeatIds = allPassengers.Select(p => p.SeatId).ToList();

            if (requestedSeatIds.Distinct().Count() != requestedSeatIds.Count)
                throw new BadRequestException("Duplicate seats in the same booking.");

            var nationalIds = allPassengers.Select(p => p.NationalId.Trim()).ToList();

            if (nationalIds.Distinct(StringComparer.OrdinalIgnoreCase).Count() != nationalIds.Count)
                throw new BadRequestException("Duplicate national IDs in the same booking.");

            var scheduleRepo = unitOfWork.GetRepository<Schedule, int>();
            var schedule = await scheduleRepo
                .GetQueryable(s => s.Id == dto.ScheduleId && !s.IsDeleted)
                .Include(s => s.Train)
                    .ThenInclude(t => t.Carriages)
                        .ThenInclude(c => c.Seats)
                .Include(s => s.Route)
                    .ThenInclude(r => r.RouteStations)
                        .ThenInclude(rs => rs.Station)
                .Include(s => s.SourceStation)
                .Include(s => s.DestinationStation)
                .FirstOrDefaultAsync();

            if (schedule is null)
                throw new NotFoundException("Schedule", dto.ScheduleId);

            var allSeats = schedule.Train.Carriages
                .SelectMany(c => c.Seats.Select(seat => new
                {
                    Seat = seat,
                    Carriage = c
                }))
                .ToList();

            var allTrainSeatIds = allSeats
                .Select(x => x.Seat.Id)
                .ToHashSet();

            var invalidSeatId = requestedSeatIds.FirstOrDefault(id => !allTrainSeatIds.Contains(id));
            if (invalidSeatId != 0)
                throw new BadRequestException($"Seat {invalidSeatId} does not belong to this train.");

            var cutoff = DateTime.UtcNow.AddMinutes(-PendingBookingExpirationMinutes);

            var ticketRepo = unitOfWork.GetRepository<Ticket, int>();

            var alreadyBookedSeatIds = await ticketRepo
                .GetQueryable(t =>
                    t.Booking.ScheduleId == dto.ScheduleId &&
                    !t.Booking.IsDeleted &&
                    (
                        t.Booking.Status == BookingStatus.Confirmed ||
                        (t.Booking.Status == BookingStatus.Pending && t.Booking.BookingDate >= cutoff)
                    ))
                .Select(t => t.SeatId)
                .ToListAsync();

            var conflictingSeat = requestedSeatIds.FirstOrDefault(id => alreadyBookedSeatIds.Contains(id));
            if (conflictingSeat != 0)
                throw new BadRequestException($"Seat {conflictingSeat} is already booked.");

            var intermediateStops = schedule.Route.RouteStations
                .OrderBy(rs => rs.Sequence)
                .Select(rs => rs.Station.Name!)
                .Where(name =>
                    name != schedule.SourceStation!.Name &&
                    name != schedule.DestinationStation!.Name)
                .ToList();

            var bookingRepo = unitOfWork.GetRepository<TrainEase.Core.Domain.Entities.Booking, int>();
            var passengerRepo = unitOfWork.GetRepository<Passenger, int>();
            var ticketWriteRepo = unitOfWork.GetRepository<TrainEase.Core.Domain.Entities.Ticket, int>();

            var booking = new TrainEase.Core.Domain.Entities.Booking
            {
                ScheduleId = dto.ScheduleId,
                UserId = userId,
                BookingDate = DateTime.UtcNow,
                Status = BookingStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await bookingRepo.AddAsync(booking);
            await unitOfWork.CompeleteAsync();

            var ticketResults = new List<TicketResultDto>();
            decimal totalAmount = 0m;

            foreach (var p in allPassengers)
            {
                var passenger = await passengerRepo
                    .GetQueryable(x => x.NationalId == p.NationalId && !x.IsDeleted)
                    .FirstOrDefaultAsync();

                if (passenger is null)
                {
                    passenger = new Passenger
                    {
                        Name = p.FullName.Trim(),
                        NationalId = p.NationalId.Trim(),
                        Email = string.IsNullOrWhiteSpace(p.Email)
                            ? $"{p.NationalId}@trainease.com"
                            : p.Email.Trim(),
                        PhoneNumber = p.PhoneNumber.Trim(),
                        Age = 0,
                        CreatedAt = DateTime.UtcNow
                    };

                    await passengerRepo.AddAsync(passenger);
                    await unitOfWork.CompeleteAsync();
                }
                var seatData = allSeats.First(x => x.Seat.Id == p.SeatId);
                var seat = seatData.Seat;
                var carriage = seatData.Carriage;
                var seatPrice = carriage.PricePerSeat ?? 0m;

                if (seatPrice <= 0)
                    throw new BadRequestException($"Seat price for carriage {carriage.CarriageNumber} is invalid.");

                var ticket = new Ticket
                {
                    BookingId = booking.Id!,
                    PassengerId = passenger.Id!,
                    SeatId = seat.Id!,
                    UserId = userId,
                    Price = seatPrice,
                    CreatedAt = DateTime.UtcNow
                };

                await ticketWriteRepo.AddAsync(ticket);
                await unitOfWork.CompeleteAsync();

                totalAmount += seatPrice;

                ticketResults.Add(new TicketResultDto
                {
                    TicketId = ticket.Id!,
                    PassengerName = passenger.Name,
                    SeatNumber = seat.SeatNumber!,
                    CarriageNumber = carriage.CarriageNumber!,
                    ClassType = carriage.ClassType!,
                    Price = seatPrice,
                    TrainName = schedule.Train.TrainName!,
                    TrainNumber = schedule.Train.TrainNumber!,
                    SourceStation = schedule.SourceStation!.Name!,
                    DestinationStation = schedule.DestinationStation!.Name!,
                    DepartureTime = schedule.DepartureTime,
                    ArrivalTime = schedule.ArrivalTime,
                    IntermediateStops = intermediateStops
                });
            }

            booking.TotalAmount = totalAmount;
            bookingRepo.Update(booking);

            await unitOfWork.CompeleteAsync();

            return new BookingResultDto
            {
                BookingId = booking.Id!,
                Status = booking.Status.ToString(),
                TotalAmount = booking.TotalAmount,
                BookingDate = booking.BookingDate,
                Tickets = ticketResults
            };
        }

        public async Task<IEnumerable<BookingResultDto>> GetBookingsByUserAsync(string userId)
        {
            await ExpireOldPendingBookingsAsync(userId: userId);

            var bookingRepo = unitOfWork.GetRepository<TrainEase.Core.Domain.Entities.Booking, int>();
            var bookings = await bookingRepo
                .GetQueryable(b => b.UserId == userId && !b.IsDeleted)
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.Seat)
                        .ThenInclude(s => s.Carriage)
                .Include(b => b.Schedule)
                    .ThenInclude(s => s.Train)
                .Include(b => b.Schedule)
                    .ThenInclude(s => s.SourceStation)
                .Include(b => b.Schedule)
                    .ThenInclude(s => s.DestinationStation)
                .ToListAsync();

            var bookingResults = bookings.Select(b => new BookingResultDto
            {
                BookingId = b.Id!,
                Status = b.Status.ToString(),
                TotalAmount = b.TotalAmount,
                BookingDate = b.BookingDate,
                Tickets = b.Tickets.Select(t => new TicketResultDto
                {
                    TicketId = t.Id!,
                    PassengerName = t.Passenger!.Name,
                    SeatNumber = t.Seat!.SeatNumber!,
                    CarriageNumber = t.Seat.Carriage!.CarriageNumber!,
                    ClassType = t.Seat.Carriage.ClassType!,
                    Price = t.Price,
                    TrainName = t.Seat.Carriage.Train!.TrainName!,
                    TrainNumber = t.Seat.Carriage.Train.TrainNumber!,
                    SourceStation = t.Seat.Carriage.Train.Schedules.Select(s => s.SourceStation!.Name!).FirstOrDefault()!,
                    DestinationStation = t.Seat.Carriage.Train.Schedules.Select(s => s.DestinationStation!.Name!).FirstOrDefault()!,
                    DepartureTime = t.Seat.Carriage.Train.Schedules.Select(s => s.DepartureTime).FirstOrDefault(),
                    ArrivalTime = t.Seat.Carriage.Train.Schedules.Select(s => s.ArrivalTime).FirstOrDefault(),
                    IntermediateStops = t.Seat.Carriage.Train.Schedules.Select(s => s.Route.RouteStations
                        .OrderBy(rs => rs.Sequence)
                        .Select(rs => rs.Station.Name!)
                        .Where(name =>
                            name != s.SourceStation!.Name &&
                            name != s.DestinationStation!.Name)
                        .ToList())
                        .FirstOrDefault() ?? new List<string>()
                }).ToList()
            });

            return bookingResults;
        }

        public async Task<IEnumerable<BookingResultDto>> GetAllBookingsAsync()
        {
            await ExpireOldPendingBookingsAsync();

            var bookingRepo = unitOfWork.GetRepository<TrainEase.Core.Domain.Entities.Booking, int>();
            var bookings = await bookingRepo
                .GetQueryable(b => !b.IsDeleted)
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.Seat)
                        .ThenInclude(s => s.Carriage)
                .Include(b => b.Schedule)
                    .ThenInclude(s => s.Train)
                .Include(b => b.Schedule)
                    .ThenInclude(s => s.SourceStation)
                .Include(b => b.Schedule)
                    .ThenInclude(s => s.DestinationStation)
                .ToListAsync();

            var bookingResults = bookings.Select(b => new BookingResultDto
            {
                BookingDate = b.BookingDate,
                BookingId = b.Id!,
                Status = b.Status.ToString(),
                TotalAmount = b.TotalAmount,
                Tickets = b.Tickets.Select(t => new TicketResultDto
                {
                    TicketId = t.Id!,
                    PassengerName = t.Passenger!.Name,
                    SeatNumber = t.Seat!.SeatNumber!,
                    CarriageNumber = t.Seat.Carriage!.CarriageNumber!,
                    ClassType = t.Seat.Carriage.ClassType!,
                    Price = t.Price,
                    TrainName = t.Seat.Carriage.Train!.TrainName!,
                    TrainNumber = t.Seat.Carriage.Train.TrainNumber!,
                    SourceStation = t.Seat.Carriage.Train.Schedules.Select(s => s.SourceStation!.Name!).FirstOrDefault()!,
                    DestinationStation = t.Seat.Carriage.Train.Schedules.Select(s => s.DestinationStation!.Name!).FirstOrDefault()!,
                    DepartureTime = t.Seat.Carriage.Train.Schedules.Select(s => s.DepartureTime).FirstOrDefault(),
                    ArrivalTime = t.Seat.Carriage.Train.Schedules.Select(s => s.ArrivalTime).FirstOrDefault(),
                    IntermediateStops = t.Seat.Carriage.Train.Schedules.Select(s => s.Route.RouteStations
                        .OrderBy(rs => rs.Sequence)
                        .Select(rs => rs.Station.Name!)
                        .Where(name =>
                            name != s.SourceStation!.Name &&
                            name != s.DestinationStation!.Name)
                        .ToList())
                        .FirstOrDefault() ?? new List<string>()
                }).ToList()
            }).ToList();

            return bookingResults;
        }

        public async Task<CancelBookingResultDto> CancelBookingAsync(int bookingId, string userId)
        {
            var bookingRepo = unitOfWork.GetRepository<TrainEase.Core.Domain.Entities.Booking, int>();
            var paymentRepo = unitOfWork.GetRepository<TrainEase.Core.Domain.Entities.Payment, int>();
            var refundRepo = unitOfWork.GetRepository<TrainEase.Core.Domain.Entities.Refund, int>();

            var booking = await bookingRepo
                .GetQueryable(b => b.Id == bookingId && !b.IsDeleted)
                .Include(b => b.Schedule)
                .Include(b => b.Payment)
                .Include(b => b.Refund)
                .Include(b => b.User)      // ✅ عشان نجيب الإيميل
                .FirstOrDefaultAsync();

            if (booking is null)
                throw new NotFoundException("Booking", bookingId);

            if (booking.UserId != userId)
                throw new UnAuthorizedException("This booking does not belong to you.");

            if (booking.Status == BookingStatus.Cancelled)
                throw new BadRequestException("This booking is already cancelled.");

            if (booking.Status == BookingStatus.Refunded)
                throw new BadRequestException("This booking is already refunded.");

            if (booking.Schedule is null)
                throw new BadRequestException("Booking schedule was not found.");

            var cancellationDate = DateTime.UtcNow;
            decimal refundAmount = 0m;
            decimal penaltyAmount = 0m;
            string message = "Booking cancelled successfully.";

            if (booking.Status == BookingStatus.Pending)
            {
                booking.Status = BookingStatus.Cancelled;

                if (booking.Payment is not null && booking.Payment.Status == PaymentStatus.Pending)
                {
                    booking.Payment.Status = PaymentStatus.Failed;
                    booking.Payment.UpdatedAt = cancellationDate;
                    paymentRepo.Update(booking.Payment);
                }
            }
            else if (booking.Status == BookingStatus.Confirmed)
            {
                if (booking.Payment is null || booking.Payment.Status != PaymentStatus.Paid)
                    throw new BadRequestException("Confirmed booking must have a paid payment.");

                if (booking.Refund is not null)
                    throw new BadRequestException("Refund record already exists for this booking.");

                var refundPercentage = GetRefundPercentage(booking.Schedule.DepartureTime);
                refundAmount = Math.Round(booking.TotalAmount * refundPercentage, 2);
                penaltyAmount = booking.TotalAmount - refundAmount;

                var refund = new Refund
                {
                    BookingId = booking.Id,
                    RefundAmount = refundAmount,
                    CancellationDate = cancellationDate,
                    CreatedAt = cancellationDate
                };

                await refundRepo.AddAsync(refund);

                booking.Status = BookingStatus.Refunded;
                booking.Payment.Status = PaymentStatus.Refunded;
                booking.Payment.RefundedAmount = refundAmount;
                booking.Payment.UpdatedAt = cancellationDate;

                paymentRepo.Update(booking.Payment);

                message = $"تم إلغاء الحجز ورد مبلغ {refundAmount} جنيه داخل النظام.";
            }
            else
            {
                throw new BadRequestException("Only pending or confirmed bookings can be cancelled.");
            }

            booking.UpdatedAt = cancellationDate;
            bookingRepo.Update(booking);

            var saved = await unitOfWork.CompeleteAsync() > 0;
            if (!saved)
                throw new BadRequestException("Failed to cancel booking.");

            // ✅ بعت إيميل الإلغاء بعد ما الداتا اتحفظت
            if (!string.IsNullOrWhiteSpace(booking.User?.Email))
            {
                var emailBody = BuildCancellationEmailBody(
                    userName: booking.User.FullName ?? "عزيزنا العميل",
                    bookingId: booking.Id,
                    originalAmount: booking.TotalAmount,
                    refundAmount: refundAmount,
                    penaltyAmount: penaltyAmount,
                    bookingStatus: booking.Status,
                    cancellationDate: cancellationDate,
                    departureTime: booking.Schedule.DepartureTime
                );

                // بنبعت الإيميل بدون await عشان ما نخليش المستخدم يستنى
                // ولو فشل الإيميل، الـ cancel اتم بنجاح
                _ = emailService.SendEmailAsync(
                    to: booking.User.Email,
                    subject: "تم إلغاء حجزك — TrainEase 🚆",
                    body: emailBody
                );
            }

            return new CancelBookingResultDto
            {
                BookingId = booking.Id,
                BookingStatus = booking.Status.ToString(),
                OriginalAmount = booking.TotalAmount,
                RefundAmount = refundAmount,
                PenaltyAmount = penaltyAmount,
                CancellationDate = cancellationDate,
                Message = message
            };
        }
        public async Task<BookingResultDto> GetBookingAsync(int id)
        {
            var bookingRepo = unitOfWork.GetRepository<TrainEase.Core.Domain.Entities.Booking, int>();
            var booking = await bookingRepo.GetAsync(id);
            var result = new BookingResultDto
            {
                BookingId = booking.Id!,
                Status = booking.Status.ToString(),
                TotalAmount = booking.TotalAmount,
                BookingDate = booking.BookingDate,
                Tickets = booking.Tickets.Select(t => new TicketResultDto
                {
                    TicketId = t.Id!,
                    PassengerName = t.Passenger!.Name,
                    SeatNumber = t.Seat!.SeatNumber!,
                    CarriageNumber = t.Seat.Carriage!.CarriageNumber!,
                    ClassType = t.Seat.Carriage.ClassType!,
                    Price = t.Price,
                    TrainName = t.Seat.Carriage.Train!.TrainName!,
                    TrainNumber = t.Seat.Carriage.Train.TrainNumber!,
                    SourceStation = t.Seat.Carriage.Train.Schedules.Select(s => s.SourceStation!.Name!).FirstOrDefault()!,
                    DestinationStation = t.Seat.Carriage.Train.Schedules.Select(s => s.DestinationStation!.Name!).FirstOrDefault()!,
                    DepartureTime = t.Seat.Carriage.Train.Schedules.Select(s => s.DepartureTime).FirstOrDefault(),
                    ArrivalTime = t.Seat.Carriage.Train.Schedules.Select(s => s.ArrivalTime).FirstOrDefault(),
                    IntermediateStops = t.Seat.Carriage.Train.Schedules.Select(s => s.Route.RouteStations
                        .OrderBy(rs => rs.Sequence)
                        .Select(rs => rs.Station.Name!)
                        .Where(name =>
                            name != s.SourceStation!.Name &&
                            name != s.DestinationStation!.Name)
                        .ToList())
                        .FirstOrDefault() ?? new List<string>()
                }).ToList()
            };
            return result;

        }
        private async Task ExpireOldPendingBookingsAsync(int? scheduleId = null, string? userId = null)
        {
            var bookingRepo = unitOfWork.GetRepository<TrainEase.Core.Domain.Entities.Booking, int>();
            var paymentRepo = unitOfWork.GetRepository<TrainEase.Core.Domain.Entities.Payment, int>();

            var cutoff = DateTime.UtcNow.AddMinutes(-PendingBookingExpirationMinutes);

            var query = bookingRepo
                .GetQueryable(b =>
                    !b.IsDeleted &&
                    b.Status == BookingStatus.Pending &&
                    b.BookingDate < cutoff)
                .Include(b => b.Payment)
                .AsQueryable();

            if (scheduleId.HasValue)
                query = query.Where(b => b.ScheduleId == scheduleId.Value);

            if (!string.IsNullOrWhiteSpace(userId))
                query = query.Where(b => b.UserId == userId);

            var expiredBookings = await query.ToListAsync();

            if (!expiredBookings.Any())
                return;

            foreach (var booking in expiredBookings)
            {
                booking.Status = BookingStatus.Cancelled;
                booking.UpdatedAt = DateTime.UtcNow;
                bookingRepo.Update(booking);

                if (booking.Payment is not null && booking.Payment.Status == PaymentStatus.Pending)
                {
                    booking.Payment.Status = PaymentStatus.Failed;
                    booking.Payment.UpdatedAt = DateTime.UtcNow;
                    paymentRepo.Update(booking.Payment);
                }
            }

            await unitOfWork.CompeleteAsync();
        }
        private static decimal GetRefundPercentage(DateTime departureTime)
        {
            var hoursBeforeDeparture = (departureTime - DateTime.UtcNow).TotalHours;

            if (hoursBeforeDeparture >= 48)
                return 0.90m;

            if (hoursBeforeDeparture >= 12)
                return 0.75m;

            if (hoursBeforeDeparture >= 4)
                return 0.50m;

            if (hoursBeforeDeparture > 0)
                return 0.25m;

            return 0m;
        }
        private static string BuildCancellationEmailBody(
    string userName,
    int bookingId,
    decimal originalAmount,
    decimal refundAmount,
    decimal penaltyAmount,
    BookingStatus bookingStatus,
    DateTime cancellationDate,
    DateTime departureTime)
        {
            var isRefunded = bookingStatus == BookingStatus.Refunded;

            var refundSection = isRefunded
                ? $"""
          <tr>
            <td style="padding:8px 0; color:#555;">المبلغ الأصلي</td>
            <td style="padding:8px 0; font-weight:bold; color:#222;">{originalAmount:N2} جنيه</td>
          </tr>
          <tr>
            <td style="padding:8px 0; color:#555;">غرامة الإلغاء</td>
            <td style="padding:8px 0; font-weight:bold; color:#e74c3c;">{penaltyAmount:N2} جنيه</td>
          </tr>
          <tr>
            <td style="padding:8px 0; color:#555;">المبلغ المسترد (داخل النظام)</td>
            <td style="padding:8px 0; font-weight:bold; color:#27ae60;">{refundAmount:N2} جنيه</td>
          </tr>
          """
                : $"""
          <tr>
            <td style="padding:8px 0; color:#555;">حالة الدفع</td>
            <td style="padding:8px 0; font-weight:bold; color:#e67e22;">لم يتم الدفع</td>
          </tr>
          """;

            return $"""
        <!DOCTYPE html>
        <html dir="rtl" lang="ar">
        <head>
          <meta charset="UTF-8"/>
          <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
        </head>
        <body style="margin:0; padding:0; background:#f4f6f8; font-family:Arial, sans-serif;">
          <table width="100%" cellpadding="0" cellspacing="0" style="background:#f4f6f8; padding:40px 0;">
            <tr>
              <td align="center">
                <table width="600" cellpadding="0" cellspacing="0"
                       style="background:#ffffff; border-radius:12px; overflow:hidden;
                              box-shadow:0 2px 8px rgba(0,0,0,0.08);">

                  <!-- Header -->
                  <tr>
                    <td style="background:#c0392b; padding:32px; text-align:center;">
                      <h1 style="margin:0; color:#ffffff; font-size:24px;">🚆 TrainEase</h1>
                      <p style="margin:8px 0 0; color:#f5b7b1; font-size:14px;">نظام حجز القطارات</p>
                    </td>
                  </tr>

                  <!-- Body -->
                  <tr>
                    <td style="padding:36px 40px;">
                      <h2 style="color:#c0392b; margin:0 0 8px;">تم إلغاء حجزك</h2>
                      <p style="color:#555; font-size:15px; margin:0 0 24px;">
                        مرحباً <strong>{userName}</strong>،<br/>
                        نود إعلامك بأنه تم إلغاء الحجز رقم <strong>#{bookingId}</strong> بنجاح.
                      </p>

                      <!-- Details Table -->
                      <table width="100%" cellpadding="0" cellspacing="0"
                             style="background:#fdf2f2; border-radius:8px; padding:20px 24px;
                                    border:1px solid #f5c6cb;">
                        <tr>
                          <td style="padding:8px 0; color:#555;">رقم الحجز</td>
                          <td style="padding:8px 0; font-weight:bold; color:#222;">#{bookingId}</td>
                        </tr>
                        <tr>
                          <td style="padding:8px 0; color:#555;">تاريخ الإلغاء</td>
                          <td style="padding:8px 0; font-weight:bold; color:#222;">
                            {cancellationDate:dd/MM/yyyy hh:mm tt}
                          </td>
                        </tr>
                        <tr>
                          <td style="padding:8px 0; color:#555;">موعد الرحلة</td>
                          <td style="padding:8px 0; font-weight:bold; color:#222;">
                            {departureTime:dd/MM/yyyy hh:mm tt}
                          </td>
                        </tr>
                        {refundSection}
                      </table>

                      <!-- Refund Note (only if refunded) -->
                      {(isRefunded ? $"""
                      <p style="margin:24px 0 0; padding:16px; background:#eafaf1; border-radius:8px;
                                 border-right:4px solid #27ae60; color:#1e8449; font-size:14px;">
                        💡 ملاحظة: المبلغ المسترد ({refundAmount:N2} جنيه) تم تسجيله داخل النظام.
                        سيتواصل معك فريق TrainEase لإتمام الإجراءات.
                      </p>
                      """ : "")}

                      <p style="margin:24px 0 0; color:#888; font-size:13px;">
                        إذا لم تقم بهذا الإلغاء أو كان هناك خطأ، يرجى التواصل معنا فوراً.
                      </p>
                    </td>
                  </tr>

                  <!-- Footer -->
                  <tr>
                    <td style="background:#f8f9fa; padding:20px 40px; text-align:center;
                               border-top:1px solid #dee2e6;">
                      <p style="margin:0; color:#aaa; font-size:12px;">
                        © {DateTime.UtcNow.Year} TrainEase — جميع الحقوق محفوظة
                      </p>
                    </td>
                  </tr>

                </table>
              </td>
            </tr>
          </table>
        </body>
        </html>
        """;
        }
    }
}