using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainEase.Core.Application.Abstrations.Model.Train
{
    public class TrainDto
    {
        public string TrainNumber { get; set; }
        public string TrainName { get; set; }
        public string TrainType { get; set; }

        public int Carriages { get; set; }
    }
}
