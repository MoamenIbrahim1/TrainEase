using TrainEase.Core.Domain.Common;

namespace TrainEase.Core.Domain.Entities
{
    public class Refund : BaseAuditableEntity
    {
        public int BookingId { get; set; }
        public Booking Booking { get; set; } = null!;
        public decimal RefundAmount { get; set; }
        public DateTime CancellationDate { get; set; } = DateTime.Now;
    }
}