using TrainEase.Core.Domain.Common;

namespace TrainEase.Core.Domain.Entities
{
    public class Invoice : BaseEntity
    {
        public int BookingId { get; set; }
        public Booking Booking { get; set; } = null!;
        public string? InvoiceNumber { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public decimal NetPrice { get; set; }
    }
}