using Microsoft.EntityFrameworkCore;
using TrainEase.Core.Application.Abstrations.Model.Booking;
using TrainEase.Core.Application.Abstrations.Services;
using TrainEase.Core.Application.Exceptions;
using TrainEase.Core.Domain.Contracts;
using TrainEase.Core.Domain.Entities;
using TrainEase.Core.Domain.Entities.Enums;

namespace TrainEase.Core.Application.Services.Booking
{
    public class BookingService(IUnitOfWork unitOfWork) : IBookingService
    {
        public async Task<IEnumerable<SeatDto>> GetSeatsByScheduleAsync(int scheduleId)
        {
            var scheduleRepo = unitOfWork.GetRepository<Schedule, int>();

            var schedule = await scheduleRepo
                .GetQueryable(s => s.Id == scheduleId && !s.IsDeleted)
                .Include(s => s.Train)
                    .ThenInclude(t => t.Carriages.Where(c => !c.IsDeleted))
                        .ThenInclude(c => c.Seats.Where(s => !s.IsDeleted))
                .FirstOrDefaultAsync();

            if (schedule is null)
                throw new NotFoundException("Schedule", scheduleId);

            var ticketRepo = unitOfWork.GetRepository<Ticket, int>();
            var bookedSeatIds = await ticketRepo
                .GetQueryable(t =>
                    t.Booking.ScheduleId == scheduleId &&
                    !t.Booking.IsDeleted &&
                    t.Booking.Status != BookingStatus.Cancelled)
                .Select(t => t.SeatId)
                .ToListAsync();

            var seats = schedule.Train.Carriages
                .SelectMany(c => c.Seats.Select(s => new SeatDto
                {
                    Id = s.Id,
                    SeatNumber = s.SeatNumber!,
                    CarriageNumber = c.CarriageNumber!,
                    ClassType = c.ClassType!,
                    IsBooked = bookedSeatIds.Contains(s.Id)
                }));

            return seats;
        }

        public async Task<BookingResultDto> CreateBookingAsync(CreateBookingDto dto, string userId)
        {
            //  Validations 

            var scheduleRepo = unitOfWork.GetRepository<Schedule, int>();
            var schedule = await scheduleRepo
                .GetQueryable(s => s.Id == dto.ScheduleId && !s.IsDeleted)
                .Include(s => s.Train)
                    .ThenInclude(t => t.Carriages)
                        .ThenInclude(c => c.Seats)
                .Include(s => s.Route)
                    .ThenInclude(r => r.RouteStations.OrderBy(rs => rs.Sequence))
                        .ThenInclude(rs => rs.Station)
                .Include(s => s.SourceStation)
                .Include(s => s.DestinationStation)
                .FirstOrDefaultAsync();

            if (schedule is null)
                throw new NotFoundException("Schedule", dto.ScheduleId);

            if (dto.Passengers is null || dto.Passengers.Count == 0)
                throw new BadRequestException("At least one passenger is required.");

            var requestedSeatIds = dto.Passengers.Select(p => p.SeatId).ToList();

            if (requestedSeatIds.Distinct().Count() != requestedSeatIds.Count)
                throw new BadRequestException("Duplicate seats in the same booking.");

            var nationalIds = dto.Passengers.Select(p => p.NationalId).ToList();
            if (nationalIds.Distinct().Count() != nationalIds.Count)
                throw new BadRequestException("Duplicate national IDs in the same booking.");

            var allTrainSeatIds = schedule.Train.Carriages
                .SelectMany(c => c.Seats)
                .Select(s => s.Id)
                .ToHashSet();

            var invalidSeat = requestedSeatIds.FirstOrDefault(id => !allTrainSeatIds.Contains(id));
            if (invalidSeat != default)
                throw new BadRequestException($"Seat {invalidSeat} does not belong to this train.");

            //  Double-booking check 

            var ticketRepo = unitOfWork.GetRepository<Ticket, int>();
            var alreadyBookedSeatIds = await ticketRepo
                .GetQueryable(t =>
                    t.Booking.ScheduleId == dto.ScheduleId &&
                    !t.Booking.IsDeleted &&
                    t.Booking.Status != BookingStatus.Cancelled)
                .Select(t => t.SeatId)
                .ToListAsync();

            var conflictingSeat = requestedSeatIds
                .FirstOrDefault(id => alreadyBookedSeatIds.Contains(id));

            if (conflictingSeat != default)
                throw new BadRequestException($"Seat {conflictingSeat} is already booked.");

            //  Create Booking 

            var totalAmount = schedule.Price * dto.Passengers.Count;

            var booking = new TrainEase.Core.Domain.Entities.Booking
            {
                ScheduleId = dto.ScheduleId,
                UserId = userId,
                BookingDate = DateTime.UtcNow,
                Status = BookingStatus.Pending,
                TotalAmount = totalAmount,
                CreatedAt = DateTime.UtcNow
            };

            var bookingRepo = unitOfWork.GetRepository<TrainEase.Core.Domain.Entities.Booking, int>();
            await bookingRepo.AddAsync(booking);
            await unitOfWork.CompeleteAsync();

            //محطات الوقوف 

            var intermediateStops = schedule.Route.RouteStations
                .OrderBy(rs => rs.Sequence)
                .Select(rs => rs.Station.Name!)
                .Where(name =>
                    name != schedule.SourceStation!.Name &&
                    name != schedule.DestinationStation!.Name)
                .ToList();

            // ── Passengers + Tickets + BookTickets ───────────────────

            var passengerRepo = unitOfWork.GetRepository<Passenger, int>();
            var ticketWriteRepo = unitOfWork.GetRepository<Ticket, int>();
            var bookTicketRepo = unitOfWork.GetRepository<BookTicket, int>();

            var ticketResults = new List<TicketResultDto>();

            foreach (var p in dto.Passengers)
            {
                // لو الـ passenger موجود بالـ NationalId استخدمه، لو لأ اعمل جديد
                var passenger = await passengerRepo
                    .GetQueryable(x => x.NationalId == p.NationalId && !x.IsDeleted)
                    .FirstOrDefaultAsync();

                if (passenger is null)
                {
                    passenger = new Passenger
                    {
                        Name = p.FullName,
                        NationalId = p.NationalId,
                        Email = p.Email ?? $"{p.NationalId}@trainease.com",
                        PhoneNumber = p.PhoneNumber,
                        Age = 0,
                        Password = "N/A",
                        CreatedAt = DateTime.UtcNow
                    };
                    await passengerRepo.AddAsync(passenger);
                    await unitOfWork.CompeleteAsync();
                }

                var carriage = schedule.Train.Carriages
                    .First(c => c.Seats.Any(s => s.Id == p.SeatId));
                var seat = carriage.Seats.First(s => s.Id == p.SeatId);

                var ticket = new Ticket
                {
                    BookingId = booking.Id,
                    SeatId = p.SeatId,
                    UserId = userId,
                    Price = schedule.Price,
                    CreatedAt = DateTime.UtcNow
                };
                await ticketWriteRepo.AddAsync(ticket);
                await unitOfWork.CompeleteAsync();

                var bookTicket = new BookTicket
                {
                    TicketId = ticket.Id,
                    PassengerId = passenger.Id!,
                    Price = schedule.Price,
                    CreatedOn = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };
                await bookTicketRepo.AddAsync(bookTicket);
                await unitOfWork.CompeleteAsync();

                ticketResults.Add(new TicketResultDto
                {
                    TicketId = ticket.Id!,
                    PassengerName = passenger.Name,
                    SeatNumber = seat.SeatNumber!,
                    CarriageNumber = carriage.CarriageNumber!,
                    ClassType = carriage.ClassType!,
                    Price = schedule.Price,
                    TrainName = schedule.Train.TrainName!,
                    TrainNumber = schedule.Train.TrainNumber!,
                    SourceStation = schedule.SourceStation!.Name!,
                    DestinationStation = schedule.DestinationStation!.Name!,
                    DepartureTime = schedule.DepartureTime,
                    ArrivalTime = schedule.ArrivalTime,
                    IntermediateStops = intermediateStops
                });
            }

            return new BookingResultDto
            {
                BookingId = booking.Id!,
                Status = "Pending",
                TotalAmount = totalAmount,
                BookingDate = booking.BookingDate,
                Tickets = ticketResults
            };
        }
    }
}