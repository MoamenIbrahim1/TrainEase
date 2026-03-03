using TrainEase.Core.Domain.Common;
using TrainEase.Core.Domain.Entities.Enums;

namespace TrainEase.Core.Domain.Entities
{
    public class Payment : BaseAuditableEntity<int>
    {
        public int BookingId { get; set; }
        public virtual Booking Booking { get; set; } = null!;
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; } 
        public DateTime PaymentDate { get; set; }
    }

}