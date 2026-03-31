using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Application.Abstrations.Model.Carriage;

namespace TrainEase.Core.Application.Abstrations.Model.Journey
{
    public class JourneySeatsDto
    {
        public int TrainId { get; set; }
        public string TrainNumber { get; set; } = null!;
        public string TrainName { get; set; } = null!;
        public List<CarriageSeatsDto> Carriages { get; set; } = new();
    }
}
