using TrainEase.Core.Domain.Common;

namespace TrainEase.Core.Domain.Entities
{
    public class Station : BaseAuditableEntity<int>
    {
        public string? Name { get; set; }
        public string? City { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public virtual ICollection<RouteStation> RouteStations { get; set; } = new HashSet<RouteStation>();
    }
}