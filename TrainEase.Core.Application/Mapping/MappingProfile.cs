using AutoMapper;
using TrainEase.Core.Application.Abstrations.Model.Journey;
using TrainEase.Core.Application.Abstrations.Model.Station;
using TrainEase.Core.Application.Abstrations.Model.Train;
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

            #region Journy Mapping
            CreateMap<Schedule, JourneySearchResultDto>()
        .ForMember(dest => dest.ScheduleId,
            opt => opt.MapFrom(src => src.Id))

        .ForMember(dest => dest.TrainNumber,
            opt => opt.MapFrom(src => src.Train.TrainNumber))

        .ForMember(dest => dest.TrainName,
            opt => opt.MapFrom(src => src.Train.TrainName))

        .ForMember(dest => dest.TrainType,
            opt => opt.MapFrom(src => src.Train.Type))

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
                (int)(src.ArrivalTime - src.DepartureTime).TotalMinutes))

        .ForMember(dest => dest.IntermediateStops,
            opt => opt.MapFrom(src =>
                src.Route.RouteStations
                    .OrderBy(rs => rs.Sequence)
                    .Select(rs => rs.Station.Name!)
                    .Where(name =>
                        name != src.SourceStation.Name &&
                        name != src.DestinationStation.Name)
                    .ToList()));
            CreateMap<JourneyCreateDto, Schedule>()
           .ForMember(dest => dest.Id, opt => opt.Ignore())
           .ForMember(dest => dest.RouteId, opt => opt.Ignore())
           .ForMember(dest => dest.Route, opt => opt.Ignore())
           .ForMember(dest => dest.Train, opt => opt.Ignore())
           .ForMember(dest => dest.SourceStation, opt => opt.Ignore())
           .ForMember(dest => dest.DestinationStation, opt => opt.Ignore())
           .ForMember(dest => dest.Price, opt => opt.Ignore())
           .ForSourceMember(src => src.IntermediateStationIds, opt => opt.DoNotValidate());



            #endregion

            //Train Mappings
            CreateMap<Train, TrainDto>()
                .ForMember(dest => dest.Id,
                   opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.TrainNumber,
                   opt => opt.MapFrom(src => src.TrainNumber))
               .ForMember(dest => dest.TrainName,
                   opt => opt.MapFrom(src => src.TrainName))
               .ForMember(dest => dest.TrainType,
                   opt => opt.MapFrom(src => src.Type));
        }
    }
}
