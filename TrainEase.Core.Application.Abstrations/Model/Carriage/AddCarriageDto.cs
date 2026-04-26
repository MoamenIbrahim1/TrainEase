using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainEase.Core.Application.Abstrations.Model.Carriage
{
    public class AddCarriageDto
    {
        public required string CarriageNumber { get; set; }
        public required string ClassType { get; set; }
        public required int SeatCount { get; set; }
    }
}
