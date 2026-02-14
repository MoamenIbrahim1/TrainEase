using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Domain.Common;

namespace TrainEase.Core.Domain.Entities
{
    public class Station : BaseEntity
    {
        public string? Name { get; set; }
        public string? City { get; set; }
        public ICollection<RouteStation> RouteStations { get; set; } = new HashSet<RouteStation>();
    }
}
