using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Application.Abstrations.Model.Journey;
using TrainEase.Core.Application.Abstrations.Model.Station;
using TrainEase.Core.Domain.Entities;

namespace TrainEase.Core.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            // Station Mappings
            CreateMap<Station, StationToReturnDto>();

            // Journey Mappings
            CreateMap<Schedule, JourneySearchResultDto>()
            .ForMember(dest => dest.ScheduleId,
                opt => opt.MapFrom(src => src.Id))

            .ForMember(dest => dest.TrainNumber,
                opt => opt.MapFrom(src => src.Train.TrainNumber))

            .ForMember(dest => dest.SourceStationName,
                opt => opt.MapFrom(src => src.SourceStation.Name))

            .ForMember(dest => dest.DestinationStationName,
                opt => opt.MapFrom(src => src.DestinationStation.Name))

            .ForMember(dest => dest.DepartureDateTime,
                opt => opt.MapFrom(src => src.DepartureTime))

            .ForMember(dest => dest.ArrivalDateTime,
                opt => opt.MapFrom(src => src.ArrivalTime))

            .ForMember(dest => dest.DurationInMinutes,
                opt => opt.MapFrom(src =>
                    (int)(src.ArrivalTime - src.DepartureTime).TotalMinutes));
        }
    }
}
