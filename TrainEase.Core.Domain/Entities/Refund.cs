using TrainEase.Core.Domain.Common;

namespace TrainEase.Core.Domain.Entities
{
    public class Refund : BaseAuditableEntity<int>
    {
        public int BookingId { get; set; }
        public virtual Booking Booking { get; set; } = null!;
        public decimal RefundAmount { get; set; }
        public DateTime CancellationDate { get; set; } = DateTime.Now;
    }
}