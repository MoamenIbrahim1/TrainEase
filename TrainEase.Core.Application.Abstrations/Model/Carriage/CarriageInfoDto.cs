using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainEase.Core.Application.Abstrations.Model.Carriage
{
    public class CarriageInfoDto
    {
        public int CarriageId { get; set; }
        public string CarriageNumber { get; set; } = null!;
        public string ClassType { get; set; } = null!;   // "First", "Second", "VIP", "Special", ...
        public decimal PricePerSeat { get; set; }
        public int TotalSeats { get; set; }
    }
}
