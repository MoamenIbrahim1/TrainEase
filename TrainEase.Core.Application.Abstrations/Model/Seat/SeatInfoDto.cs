using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainEase.Core.Application.Abstrations.Model.Seat
{
    public class SeatInfoDto
    {
        public int SeatId { get; set; }
        public string SeatNumber { get; set; } = null!;
        public bool IsBooked { get; set; }
    }
}
