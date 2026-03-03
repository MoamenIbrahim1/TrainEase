using TrainEase.Core.Domain.Common;

namespace TrainEase.Core.Domain.Entities
{
    public class Route : BaseAuditableEntity<int>
    {
        public string? RouteName { get; set; }
        public virtual ICollection<RouteStation> RouteStations { get; set; } = new HashSet<RouteStation>();
        public virtual ICollection<Schedule> Schedules { get; set; } = new HashSet<Schedule>();
    }
}
