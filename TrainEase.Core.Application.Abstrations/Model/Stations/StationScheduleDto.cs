using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainEase.Core.Application.Abstrations.Model.Stations
{
    public class StationScheduleDto
    {
        public int ScheduleId { get; set; }
        public string TrainNumber { get; set; } = null!;
        public string TrainName { get; set; } = null!;
        public string DestinationStation { get; set; } = null!;
        public DateTime DepartureTime { get; set; }
    }
}
