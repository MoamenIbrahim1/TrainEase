using TrainEase.Core.Application.Abstrations.Model.Carriage;

namespace TrainEase.Core.Application.Abstrations.Model.Journey
{
    public class JourneyCreateDto
    {
        public int TrainId { get; set; }
        public int SourceStationId { get; set; }
        public int DestinationStationId { get; set; }
        public List<int> IntermediateStationIds { get; set; } = new();
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }

        // ✅ الأسعار بتيجي per carriage مش per class
        public List<ClassPriceDto> CarriagePrices { get; set; } = new();
    }

   
}