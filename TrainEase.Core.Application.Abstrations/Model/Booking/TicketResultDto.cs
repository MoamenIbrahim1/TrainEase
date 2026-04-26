using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainEase.Core.Application.Abstrations.Model.Booking
{
    public class TicketResultDto
    {
        public int TicketId { get; set; }
        public string PassengerName { get; set; } = null!;
        public string SeatNumber { get; set; } = null!;
        public string CarriageNumber { get; set; } = null!;
        public string ClassType { get; set; } = null!;       
        public decimal? Price { get; set; }

        // Train info
        public string TrainName { get; set; } = null!;
        public string TrainNumber { get; set; } = null!;

        // Journey info
        public string SourceStation { get; set; } = null!;
        public string DestinationStation { get; set; } = null!;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }

        // Stops
        public List<string> IntermediateStops { get; set; } = new();
    }
}
