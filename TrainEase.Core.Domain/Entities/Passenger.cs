using TrainEase.Core.Domain.Common;

namespace TrainEase.Core.Domain.Entities
{
    public class Passenger : BaseAuditableEntity<int>
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public int Age { get; set; }
        public required string NationalId { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}