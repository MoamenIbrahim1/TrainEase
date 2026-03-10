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
    public class JourneyService(IUnitOfWork _unitOfWork,IMapper _mapper) : IJourneyService
    {
        public async Task<IEnumerable<JourneySearchResultDto>> GetAllJourneysAsync()
        {
           var schedulesRepo = _unitOfWork.GetRepository<Schedule, int>();
           var schedules = await schedulesRepo.GetAllAsync();
           var result = _mapper.Map<IEnumerable<JourneySearchResultDto>>(schedules);
           return result;
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

            var schedules = schedulesRepo
             .GetQueryable()
             .Where(s =>
                 s.SourceStationId == fromStationId &&
                 s.DestinationStationId == toStationId &&
                 s.DepartureTime.Date == date.Date &&
                 !s.IsDeleted)
             .Select(s => new JourneySearchResultDto
             {
                 ScheduleId = s.Id,
                 TrainNumber = s.Train.TrainNumber!,
                 SourceStationName = s.SourceStation.Name!,
                 DestinationStationName = s.DestinationStation.Name!,
                 DepartureDateTime = s.DepartureTime,
                 ArrivalDateTime = s.ArrivalTime,
                 DurationInMinutes = (int)(s.ArrivalTime - s.DepartureTime).TotalMinutes,
                 Price = s.Price
             });
            return await schedules.ToListAsync();
        }
    }
}
