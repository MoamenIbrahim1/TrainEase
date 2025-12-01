using System;
using System.Collections.Generic;
using System.Text;

namespace TrainEase.Core.Domain.Entities
{
    public class Station
    {
        public int StationId { get; set; }
        public string StationName { get; set; } = null!;
        public StationMaster StationMaster { get; set; }
    }
}
