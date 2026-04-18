using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
