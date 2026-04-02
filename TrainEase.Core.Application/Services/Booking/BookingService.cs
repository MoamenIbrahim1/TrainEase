using Microsoft.EntityFrameworkCore;
using TrainEase.Core.Application.Abstrations.Model.Booking;
using TrainEase.Core.Application.Abstrations.Model.Carriage;
using TrainEase.Core.Application.Abstrations.Model.Journey;
using TrainEase.Core.Application.Abstrations.Model.Seat;
using TrainEase.Core.Application.Abstrations.Services;
using TrainEase.Core.Application.Exceptions;
using TrainEase.Core.Domain.Contracts;
using TrainEase.Core.Domain.Entities;
using TrainEase.Core.Domain.Entities.Enums;

namespace TrainEase.Core.Application.Services.Booking
{
    public class BookingService(IUnitOfWork unitOfWork) : IBookingService
    {
        public async Task<JourneySeatsDto> GetSeatsByScheduleAsync(int scheduleId)
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

            // جيب المقاعد المحجوزة
            var ticketRepo = unitOfWork.GetRepository<Ticket, int>();
            var bookedSeatIds = await ticketRepo
                .GetQueryable(t =>
                    t.Booking.ScheduleId == scheduleId &&
                    !t.Booking.IsDeleted &&
                    t.Booking.Status != BookingStatus.Cancelled)
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

            if (dto.Passengers is null || dto.Passengers.Count == 0)
                throw new BadRequestException("At least one passenger is required.");

            var requestedSeatIds = dto.Passengers.Select(p => p.SeatId).ToList();

            if (requestedSeatIds.Distinct().Count() != requestedSeatIds.Count)
                throw new BadRequestException("Duplicate seats in the same booking.");

            var nationalIds = dto.Passengers.Select(p => p.NationalId.Trim()).ToList();

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

            var ticketRepo = unitOfWork.GetRepository<Ticket, int>();

            var alreadyBookedSeatIds = await ticketRepo
                .GetQueryable(t =>
                    t.Booking.ScheduleId == dto.ScheduleId &&
                    !t.Booking.IsDeleted &&
                    t.Booking.Status != BookingStatus.Cancelled &&
                    t.Booking.Status != BookingStatus.Refunded)
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
            var ticketWriteRepo = unitOfWork.GetRepository<Ticket, int>();

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

            foreach (var p in dto.Passengers)
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
                var seatPrice = carriage.PricePerSeat;

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
    }
}