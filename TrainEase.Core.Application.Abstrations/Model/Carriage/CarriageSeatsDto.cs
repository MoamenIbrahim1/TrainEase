using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Application.Abstrations.Model.Seat;

namespace TrainEase.Core.Application.Abstrations.Model.Carriage
{
    public class CarriageSeatsDto
    {
        public int CarriageId { get; set; }
        public string CarriageNumber { get; set; } = null!;
        public string ClassType { get; set; } = null!;
        public int SeatsCount { get; set; }
        public decimal PricePerSeat { get; set; }
        public List<SeatInfoDto> Seats { get; set; } = new();
    }
}
