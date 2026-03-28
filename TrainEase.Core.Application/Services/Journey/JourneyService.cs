using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TrainEase.Core.Application.Abstrations.Model.Journey;
using TrainEase.Core.Application.Abstrations.Services;
using TrainEase.Core.Application.Exceptions;
using TrainEase.Core.Domain.Contracts;
using TrainEase.Core.Domain.Entities;

namespace TrainEase.Core.Application.Services.Journey
{
    public class JourneyService(IUnitOfWork _unitOfWork, IMapper _mapper) : IJourneyService
    {
        public async Task<IEnumerable<JourneySearchResultDto>> GetAllJourneysAsync()
        {
            var schedulesRepo = _unitOfWork.GetRepository<Schedule, int>();

            var schedules = await schedulesRepo
                .GetQueryable(s => !s.IsDeleted)
                .Include(s => s.Train)
                .Include(s => s.SourceStation)
                .Include(s => s.DestinationStation)
                .Include(s => s.Route)
                    .ThenInclude(r => r.RouteStations.OrderBy(rs => rs.Sequence))
                        .ThenInclude(rs => rs.Station)
                .ToListAsync();

            return _mapper.Map<IEnumerable<JourneySearchResultDto>>(schedules);
        }

        public async Task<IEnumerable<JourneySearchResultDto>> SearchJourneysAsync(
            int fromStationId,
            int toStationId,
            DateTime date)
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
                .Include(s => s.SourceStation)
                .Include(s => s.DestinationStation)
                .Include(s => s.Route)
                    .ThenInclude(r => r.RouteStations.OrderBy(rs => rs.Sequence))
                        .ThenInclude(rs => rs.Station)
                .ToListAsync();

            return _mapper.Map<IEnumerable<JourneySearchResultDto>>(schedules);
        }
    }
}
