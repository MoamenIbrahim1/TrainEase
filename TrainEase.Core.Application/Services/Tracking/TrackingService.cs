using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Application.Abstrations.Model.Tracking;
using TrainEase.Core.Application.Abstrations.Services;
using TrainEase.Core.Application.Exceptions;
using TrainEase.Core.Domain.Contracts;
using TrainEase.Core.Domain.Entities;

namespace TrainEase.Core.Application.Services.Tracking
{
    public class TrackingService(IUnitOfWork _unitOfWork) : ITrackingService
    {
        public async Task<TrainTrackingDto> GetTrainTrackingAsync(int scheduleId)
        {
            var scheduleRepo = _unitOfWork.GetRepository<Schedule, int>();
            var delayRepo = _unitOfWork.GetRepository<TrainDelay, int>();

            var schedule = await scheduleRepo
                .GetQueryable(s => s.Id == scheduleId && !s.IsDeleted)
                .Include(s => s.Train)
                .Include(s => s.Route)
                    .ThenInclude(r => r.RouteStations)
                    .ThenInclude(rs => rs.Station)
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException("Schedule not found.", scheduleId.ToString());

            // ✅ تجاهل الـ records اللي DelayMinutes فيها null
            var latestDelay = await delayRepo
                .GetQueryable(d => d.ScheduleId == scheduleId && !d.IsDeleted && d.DelayMinutes != null)
                .OrderByDescending(d => d.ReportedAt)
                .FirstOrDefaultAsync();

            var totalDelayMinutes = latestDelay?.DelayMinutes ?? 0;

            var allStations = schedule.Route.RouteStations
                .OrderBy(rs => rs.Sequence)
                .Select(rs => rs.Station)
                .ToList();

            // ✅ تحقق إن فيه محطتين على الأقل
            if (allStations.Count < 2)
                throw new ValidationException { Errors = ["Route must have at least 2 stations."] };

            var now = DateTime.UtcNow;
            var actualDeparture = schedule.DepartureTime;
            var actualArrival = schedule.ArrivalTime.AddMinutes(totalDelayMinutes);

            // ✅ تحقق إن الـ ArrivalTime بعد الـ DepartureTime
            if (actualArrival <= actualDeparture)
                throw new ValidationException { Errors = ["Invalid schedule: ArrivalTime must be after DepartureTime."] };

            var firstStation = allStations.First();
            var lastStation = allStations.Last();

            // لو الرحلة لسه مبدأتش
            if (now < actualDeparture)
            {
                return new TrainTrackingDto
                {
                    ScheduleId = scheduleId,
                    TrainNumber = schedule.Train.TrainNumber,
                    CurrentSegment = $"{firstStation.Name} → {allStations[1].Name}",
                    FromStationName = firstStation.Name,        // ✅
                    ToStationName = allStations[1].Name,        // ✅
                    CurrentLatitude = firstStation.Latitude,
                    CurrentLongitude = firstStation.Longitude,
                    ProgressPercent = 0,
                    TotalDelayMinutes = totalDelayMinutes,
                    EstimatedRemainingMinutes = (int)(actualArrival - now).TotalMinutes,
                    Status = totalDelayMinutes > 0 ? "Delayed" : "NotStarted"
                };
            }

            // لو الرحلة خلصت
            if (now >= actualArrival)
            {
                return new TrainTrackingDto
                {
                    ScheduleId = scheduleId,
                    TrainNumber = schedule.Train.TrainNumber,
                    CurrentSegment = $"{allStations[^2].Name} → {lastStation.Name}",
                    FromStationName = allStations[^2].Name,     // ✅
                    ToStationName = lastStation.Name,            // ✅
                    CurrentLatitude = lastStation.Latitude,
                    CurrentLongitude = lastStation.Longitude,
                    ProgressPercent = 100,
                    TotalDelayMinutes = totalDelayMinutes,
                    EstimatedRemainingMinutes = 0,
                    Status = "Completed"
                };
            }

            // حساب المسافة
            var segmentDistances = new List<double>();
            for (int i = 0; i < allStations.Count - 1; i++)
            {
                segmentDistances.Add(CalculateDistance(
                    allStations[i].Latitude, allStations[i].Longitude,
                    allStations[i + 1].Latitude, allStations[i + 1].Longitude));
            }

            var totalDistance = segmentDistances.Sum();
            var totalDuration = (actualArrival - actualDeparture).TotalSeconds;
            var elapsed = (now - actualDeparture).TotalSeconds;
            var overallProgress = Math.Clamp(elapsed / totalDuration, 0, 1);
            var coveredDistance = overallProgress * totalDistance;

            // إيجاد الـ Segment الحالي
            double accumulated = 0;
            var fromStation = allStations[0];
            var toStation = allStations[1];
            double segmentProgress = 0;

            for (int i = 0; i < segmentDistances.Count; i++)
            {
                if (coveredDistance <= accumulated + segmentDistances[i])
                {
                    fromStation = allStations[i];
                    toStation = allStations[i + 1];
                    segmentProgress = segmentDistances[i] > 0
                        ? (coveredDistance - accumulated) / segmentDistances[i]
                        : 0;
                    break;
                }
                accumulated += segmentDistances[i];
            }

            var currentLat = fromStation.Latitude
                + (toStation.Latitude - fromStation.Latitude) * segmentProgress;
            var currentLng = fromStation.Longitude
                + (toStation.Longitude - fromStation.Longitude) * segmentProgress;

            return new TrainTrackingDto
            {
                ScheduleId = scheduleId,
                TrainNumber = schedule.Train.TrainNumber,
                CurrentSegment = $"{fromStation.Name} → {toStation.Name}",
                FromStationName = fromStation.Name,    
                ToStationName = toStation.Name,        
                CurrentLatitude = currentLat,
                CurrentLongitude = currentLng,
                ProgressPercent = Math.Round(overallProgress * 100, 2),
                TotalDelayMinutes = totalDelayMinutes,
                EstimatedRemainingMinutes = (int)(actualArrival - now).TotalMinutes,
                Status = totalDelayMinutes > 0 ? "Delayed" : "Moving"
            };
        }

        private double CalculateDistance(double lat1, double lng1, double lat2, double lng2)
        {
            const double R = 6371000;
            var dLat = (lat2 - lat1) * Math.PI / 180;
            var dLng = (lng2 - lng1) * Math.PI / 180;
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                  + Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180)
                  * Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
            return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }
    }
}