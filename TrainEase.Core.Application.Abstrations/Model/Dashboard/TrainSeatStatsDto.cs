using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainEase.Core.Application.Abstrations.Model.Dashboard
{
    public class TrainSeatStatsDto
    {
        public int ScheduleId { get; set; }
        public int TrainId { get; set; }
        public string TrainNumber { get; set; } = null!;
        public string TrainName { get; set; } = null!;
        public int TotalSeats { get; set; }
        public int BookedSeats { get; set; }
        public int AvailableSeats { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
    }
}
