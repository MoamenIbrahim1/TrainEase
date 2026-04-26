using TrainEase.Core.Application.Abstrations.Model.Carriage;

namespace TrainEase.Core.Application.Abstrations.Model.Journey
{
    public class JourneySearchResultDto
    {
        public int ScheduleId { get; set; }
        public string TrainNumber { get; set; } = null!;
        public string TrainName { get; set; } = null!;
        public string TrainType { get; set; } = null!;
        public string SourceStationName { get; set; } = null!;
        public string DestinationStationName { get; set; } = null!;
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public int DurationInMinutes { get; set; }
        public List<string> IntermediateStops { get; set; } = new();
        public List<ClassPriceDto> ClassPrices { get; set; } = new();
    }

   
}