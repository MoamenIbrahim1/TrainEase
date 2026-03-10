using TrainEase.Core.Domain.Common;

namespace TrainEase.Core.Domain.Entities
{
    public class RouteStation : BaseAuditableEntity<int>
    {
        public int Sequence { get; set; }

        public int RouteId { get; set; }
        public virtual Route Route { get; set; } = null!;
        public int StationId { get; set; }
        public virtual Station Station { get; set; } = null!;
    }
}