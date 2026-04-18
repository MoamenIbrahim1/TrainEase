using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Application.Abstrations.Model.Dashboard;
using TrainEase.Core.Application.Abstrations.Services;
using TrainEase.Core.Domain.Contracts;
using TrainEase.Core.Domain.Entities;
using TrainEase.Core.Domain.Entities.Enums;

namespace TrainEase.Core.Application.Services.Dashboard
{
    public class DashboardService(IUnitOfWork _unitOfWork) : IDashboardService
    {
        public async Task<int> GetTotalActivePassengersAsync()
        {
            var activePassengers = await _unitOfWork.GetRepository<Passenger, int>().GetAllAsync();
            return activePassengers.Count();

        }

        public async Task<int> GetTotalTrainsAsync()
        {
            var allTrains = await _unitOfWork.GetRepository<Train, int>().GetAllAsync();
            return allTrains.Count();
        }
        public async Task<int> GetTodaysBookingsAsync()
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);
            var todaysBookings = _unitOfWork.GetRepository<TrainEase.Core.Domain.Entities.Booking, int>()
                .GetQueryable().Where(b => b.BookingDate >= today && b.BookingDate < tomorrow);
            return await todaysBookings.CountAsync();
        }

        public async Task<IEnumerable<TrainSeatStatsDto>> GetAllTrainSeatStatsAsync()
        {
            var scheduleRepo = _unitOfWork.GetRepository<Schedule, int>();
            var ticketRepo = _unitOfWork.GetRepository<Ticket, int>();

            var schedules = await scheduleRepo
                .GetQueryable(s => !s.IsDeleted)
                .Include(s => s.Train)
                    .ThenInclude(t => t.Carriages)
                        .ThenInclude(c => c.Seats)
                .ToListAsync();

            var scheduleIds = schedules.Select(s => s.Id).ToList();

            var bookedSeatCounts = await ticketRepo
                .GetQueryable(t =>
                    !t.IsDeleted &&
                    scheduleIds.Contains(t.Booking.ScheduleId) &&
                    !t.Booking.IsDeleted &&
                    t.Booking.Status != BookingStatus.Cancelled &&
                    t.Booking.Status != BookingStatus.Refunded)
                .GroupBy(t => t.Booking.ScheduleId)
                .Select(g => new
                {
                    ScheduleId = g.Key,
                    BookedSeats = g.Select(x => x.SeatId).Distinct().Count()
                })
                .ToListAsync();

            var bookedLookup = bookedSeatCounts.ToDictionary(x => x.ScheduleId, x => x.BookedSeats);

            var result = schedules
                .Select(s =>
                {
                    var totalSeats = s.Train.Carriages.SelectMany(c => c.Seats).Count();
                    var bookedSeats = bookedLookup.ContainsKey(s.Id) ? bookedLookup[s.Id] : 0;

                    return new TrainSeatStatsDto
                    {
                        ScheduleId = s.Id,
                        TrainId = s.TrainId,
                        TrainNumber = s.Train.TrainNumber!,
                        TrainName = s.Train.TrainName!,
                        TotalSeats = totalSeats,
                        BookedSeats = bookedSeats,
                        AvailableSeats = totalSeats - bookedSeats,
                        DepartureTime = s.DepartureTime,
                        ArrivalTime = s.ArrivalTime
                    };
                })
                .OrderByDescending(x => x.DepartureTime)
                .ToList();

            return result;
        }
    }
}