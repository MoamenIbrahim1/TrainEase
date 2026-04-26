using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Application.Abstrations.Model.Carriage;
using TrainEase.Core.Application.Abstrations.Model.Journey;
using TrainEase.Core.Application.Abstrations.Services;
using TrainEase.Core.Application.Exceptions;
using TrainEase.Core.Domain.Contracts;
using TrainEase.Core.Domain.Entities;

namespace TrainEase.Core.Application.Services.Journey
{
    public class JourneyService(IUnitOfWork _unitOfWork, IMapper _mapper) : IJourneyService
    {
        public async Task<JourneySearchResultDto> CreateJourneyAsync(JourneyCreateDto journeyCreateDto)
        {
            if (journeyCreateDto is null)
                throw new BadRequestException("Journey data is required.");

            if (journeyCreateDto.SourceStationId == journeyCreateDto.DestinationStationId)
                throw new BadRequestException("Source and destination stations cannot be the same.");

            if (journeyCreateDto.ArrivalTime <= journeyCreateDto.DepartureTime)
                throw new BadRequestException("Arrival time must be after departure time.");

            var stationIds = new List<int> { journeyCreateDto.SourceStationId };
            stationIds.AddRange(journeyCreateDto.IntermediateStationIds);
            stationIds.Add(journeyCreateDto.DestinationStationId);

            if (stationIds.Distinct().Count() != stationIds.Count)
                throw new BadRequestException("Duplicate stations are not allowed in the route.");

            var routeRepo = _unitOfWork.GetRepository<Route, int>();
            var routeStationRepo = _unitOfWork.GetRepository<RouteStation, int>();
            var scheduleRepo = _unitOfWork.GetRepository<Schedule, int>();
            var carriageRepo = _unitOfWork.GetRepository<Carriage, int>();

            // إنشاء الـ Route
            var route = new Route { CreatedAt = DateTime.UtcNow };
            await routeRepo.AddAsync(route);
            await _unitOfWork.CompeleteAsync();

            // إضافة المحطات بالترتيب
            int sequence = 1;
            foreach (var stationId in stationIds)
            {
                await routeStationRepo.AddAsync(new RouteStation
                {
                    RouteId = route.Id,
                    StationId = stationId,
                    Sequence = sequence++,
                    CreatedAt = DateTime.UtcNow
                });
            }

            // إنشاء الـ Schedule
            var schedule = _mapper.Map<Schedule>(journeyCreateDto);
            schedule.RouteId = route.Id;
            schedule.Price = 0;
            schedule.CreatedAt = DateTime.UtcNow;

            await scheduleRepo.AddAsync(schedule);
            await _unitOfWork.CompeleteAsync();

            // ✅ تحديث أسعار العربات عن طريق ClassType
            if (journeyCreateDto.CarriagePrices.Any())
            {
                var trainCarriages = await carriageRepo
                    .GetQueryable(c => c.TrainId == journeyCreateDto.TrainId && !c.IsDeleted)
                    .ToListAsync();

                foreach (var cp in journeyCreateDto.CarriagePrices)
                {
                    if (cp.Price < 0)
                        throw new BadRequestException($"Price for class {cp.ClassType} cannot be negative.");

                    var normalizedClassType = cp.ClassType.Trim().ToLower();

                    var matchingCarriages = trainCarriages
                        .Where(c => c.ClassType != null && c.ClassType.Trim().ToLower() == normalizedClassType)
                        .ToList();

                    if (!matchingCarriages.Any())
                        throw new BadRequestException($"No carriages found with class type '{cp.ClassType}' in this train.");

                    foreach (var carriage in matchingCarriages)
                    {
                        carriage.PricePerSeat = cp.Price;
                        carriageRepo.Update(carriage);
                    }
                }

                await _unitOfWork.CompeleteAsync();
            }

            // جيب الـ Schedule كامل بكل بياناته
            var createdSchedule = await scheduleRepo
                .GetQueryable(s => s.Id == schedule.Id)
                .Include(s => s.Train)
                    .ThenInclude(t => t.Carriages.Where(c => !c.IsDeleted))
                .Include(s => s.SourceStation)
                .Include(s => s.DestinationStation)
                .Include(s => s.Route)
                    .ThenInclude(r => r.RouteStations.OrderBy(rs => rs.Sequence))
                        .ThenInclude(rs => rs.Station)
                .FirstOrDefaultAsync();

            if (createdSchedule is null)
                throw new BadRequestException("Failed to retrieve created journey.");

            // ✅ Map يدوي بسيط بدون MapTo method
            var intermediateStops = createdSchedule.Route.RouteStations
                .OrderBy(rs => rs.Sequence)
                .Select(rs => rs.Station.Name!)
                .Where(name =>
                    name != createdSchedule.SourceStation!.Name &&
                    name != createdSchedule.DestinationStation!.Name)
                .ToList();

            // ✅ CarriagePriceDto — ClassType مع سعره بدون تكرار
            var carriages = createdSchedule.Train.Carriages
                .GroupBy(c => c.ClassType)
                .Select(g => new ClassPriceDto
                {
                    ClassType = g.Key!,
                    Price = g.First().PricePerSeat ?? 0m
                })
                .ToList();

            return new JourneySearchResultDto
            {
                ScheduleId = createdSchedule.Id,
                TrainNumber = createdSchedule.Train.TrainNumber!,
                TrainName = createdSchedule.Train.TrainName!,
                TrainType = createdSchedule.Train.Type ?? "",
                SourceStationName = createdSchedule.SourceStation!.Name!,
                DestinationStationName = createdSchedule.DestinationStation!.Name!,
                DepartureDateTime = createdSchedule.DepartureTime,
                ArrivalDateTime = createdSchedule.ArrivalTime,
                DurationInMinutes = (int)(createdSchedule.ArrivalTime - createdSchedule.DepartureTime).TotalMinutes,
                IntermediateStops = intermediateStops,
                ClassPrices = carriages
            };
        }

        public async Task<IEnumerable<JourneySearchResultDto>> GetAllJourneysAsync()
        {
            var schedulesRepo = _unitOfWork.GetRepository<Schedule, int>();

            var schedules = await schedulesRepo
                .GetQueryable(s => !s.IsDeleted)
                .Include(s => s.Train)
                    .ThenInclude(t => t.Carriages.Where(c => !c.IsDeleted))
                .Include(s => s.SourceStation)
                .Include(s => s.DestinationStation)
                .Include(s => s.Route)
                    .ThenInclude(r => r.RouteStations.OrderBy(rs => rs.Sequence))
                        .ThenInclude(rs => rs.Station)
                .ToListAsync();

            // ✅ بدل _mapper.Map
            return schedules.Select(MapToDto).ToList();
        }

        public async Task<IEnumerable<JourneySearchResultDto>> SearchJourneysAsync(
            int fromStationId, int toStationId, DateTime date)
        {
            if (fromStationId <= 0)
                throw new BadRequestException("Invalid source station id.");

            if (toStationId <= 0)
                throw new BadRequestException("Invalid destination station id.");

            if (fromStationId == toStationId)
                throw new BadRequestException("Source and destination stations cannot be the same.");

            if (date < DateTime.Today)
                throw new BadRequestException("Travel date cannot be in the past.");

            var schedulesRepo = _unitOfWork.GetRepository<Schedule, int>();

            var schedules = await schedulesRepo
                .GetQueryable(s =>
                    s.SourceStationId == fromStationId &&
                    s.DestinationStationId == toStationId &&
                    s.DepartureTime.Date == date.Date &&
                    !s.IsDeleted)
                .Include(s => s.Train)
                    .ThenInclude(t => t.Carriages.Where(c => !c.IsDeleted))
                .Include(s => s.SourceStation)
                .Include(s => s.DestinationStation)
                .Include(s => s.Route)
                    .ThenInclude(r => r.RouteStations.OrderBy(rs => rs.Sequence))
                        .ThenInclude(rs => rs.Station)
                .ToListAsync();

            // ✅ بدل _mapper.Map
            return schedules.Select(MapToDto).ToList();
        }
        private static JourneySearchResultDto MapToDto(Schedule schedule)
        {
            var intermediateStops = schedule.Route.RouteStations
                .OrderBy(rs => rs.Sequence)
                .Select(rs => rs.Station.Name!)
                .Where(name =>
                    name != schedule.SourceStation!.Name &&
                    name != schedule.DestinationStation!.Name)
                .ToList();

            var classPrices = schedule.Train.Carriages
                .GroupBy(c => c.ClassType)
                .Select(g => new ClassPriceDto
                {
                    ClassType = g.Key!,
                    Price = g.First().PricePerSeat ?? 0m
                })
                .ToList();

            return new JourneySearchResultDto
            {
                ScheduleId = schedule.Id,
                TrainNumber = schedule.Train.TrainNumber!,
                TrainName = schedule.Train.TrainName!,
                TrainType = schedule.Train.Type ?? "",
                SourceStationName = schedule.SourceStation!.Name!,
                DestinationStationName = schedule.DestinationStation!.Name!,
                DepartureDateTime = schedule.DepartureTime,
                ArrivalDateTime = schedule.ArrivalTime,
                DurationInMinutes = (int)(schedule.ArrivalTime - schedule.DepartureTime).TotalMinutes,
                IntermediateStops = intermediateStops,
                ClassPrices = classPrices
            };
        }
    }
}