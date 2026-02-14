using TrainEase.Core.Domain.Common;

namespace TrainEase.Core.Domain.Entities
{
    public class Ticket : BaseEntity
    {
        public int BookingId { get; set; }
        public Booking Booking { get; set; } = null!;
        public int SeatId { get; set; }
        public Seat Seat { get; set; } = null!;
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public decimal Price { get; set; }
    }
}