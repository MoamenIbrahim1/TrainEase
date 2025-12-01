using TrainEase.Core.Domain.Common;

namespace TrainEase.Core.Domain.Entities
{
    public class Passenger : BaseEntity<int>
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Password { get; set; }
        public int Age { get; set; }
        public required string NationalId { get; set; }
        public int NotificationId { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<Ticket> PassengerTickets { get; set; } = new List<PassengerTicket>();
        public virtual Notification Notifications { get; set; } 
    }
}