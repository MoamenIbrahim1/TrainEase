using TrainEase.Core.Domain.Common;

namespace TrainEase.Core.Domain.Entities
{
    public class Carriage : BaseAuditableEntity
    {
        public int TrainId { get; set; }
        public Train Train { get; set; } = null!;
        public string? CarriageNumber { get; set; }
        public string? ClassType { get; set; } // First, Second
        public ICollection<Seat> Seats { get; set; } = new HashSet<Seat>();
    }
}