using System;

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
        public decimal FirstClassPrice { get; set; } = 250m;
        public decimal SecondClassPrice { get; set; } = 150m;
        public decimal VipPrice { get; set; } = 500m;
        public List<string> IntermediateStops { get; set; } = new();
    }
}
