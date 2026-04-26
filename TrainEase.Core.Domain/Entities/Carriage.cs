using TrainEase.Core.Domain.Common;

namespace TrainEase.Core.Domain.Entities
{
    public class Carriage : BaseAuditableEntity<int>
    {
        public int TrainId { get; set; }
        public virtual Train Train { get; set; } = null!;
        public decimal? PricePerSeat { get; set; } 
        public string? CarriageNumber { get; set; }
        public string? ClassType { get; set; } // First, Second
        public virtual ICollection<Seat> Seats { get; set; } = new HashSet<Seat>();
    }
}