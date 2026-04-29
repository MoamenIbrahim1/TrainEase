using TrainEase.Core.Domain.Common;

namespace TrainEase.Core.Domain.Entities
{
    public class Route : BaseAuditableEntity<int>
    {
        public string? RouteName { get; set; }
        public int Sequence { get; set; }
        public virtual ICollection<RouteStation> RouteStations { get; set; } = new HashSet<RouteStation>();
    }
}