using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainEase.Core.Application.Abstrations.Model.Train
{
    public class TrainDto
    {
        public int Id { get; set; }
        public string TrainNumber { get; set; } = null!;
        public string TrainName { get; set; } = null!;
        public string TrainType { get; set; } = null!;
        public int CarriagesCount { get; set; }
        public int ClassesCount { get; set; }
        public List<TrainClassDto> Classes { get; set; } = new();
    }
}
