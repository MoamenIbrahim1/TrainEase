using TrainEase.Core.Domain.Common;
using TrainEase.Core.Domain.Entities.Enums;

namespace TrainEase.Core.Domain.Entities
{
    public class User : BaseEntity
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? PhoneNumber { get; set; }
        public UserRole Role { get; set; } // Admin, Passenger, StationMaster

        
        public ICollection<Booking> Bookings { get; set; } = new HashSet<Booking>();
        public ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
    }
}