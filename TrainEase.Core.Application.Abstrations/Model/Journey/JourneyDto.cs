using System;

namespace TrainEase.Core.Application.Abstrations.Model.Journey
{
    public class JourneySearchResultDto
    {
        public int ScheduleId { get; set; }  
        public string TrainNumber { get; set; } = null!;
        public string SourceStationName { get; set; } = null!;
        public string DestinationStationName { get; set; } = null!;
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public int DurationInMinutes { get; set; }
        public decimal Price { get; set; }
    }
}
