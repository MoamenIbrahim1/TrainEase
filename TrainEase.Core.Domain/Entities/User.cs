using Microsoft.AspNetCore.Identity;
using TrainEase.Core.Domain.Entities.Enums;

namespace TrainEase.Core.Domain.Entities
{
    public class User : IdentityUser
    {
        public string? FullName { get; set; }
        public string? NationalId { get; set; }
        public UserRole Role { get; set; }
        public bool IsDeleted { get; set; } = false;
        public int? StationId { get; set; }
        public virtual Station? station { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; } = new HashSet<Booking>();
        public virtual ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
    }
}