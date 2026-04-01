using TrainEase.Core.Domain.Common;

namespace TrainEase.Core.Domain.Entities
{
    public class Ticket : BaseAuditableEntity<int>
    {
        public int BookingId { get; set; }
        public virtual Booking Booking { get; set; } = null!;

        public int PassengerId { get; set; }
        public virtual Passenger Passenger { get; set; } = null!;

        public int SeatId { get; set; }
        public virtual Seat Seat { get; set; } = null!;

        public string UserId { get; set; } = null!;
        public virtual User User { get; set; } = null!;

        public decimal Price { get; set; }
    }
}