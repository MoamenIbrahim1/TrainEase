using TrainEase.Core.Domain.Common;

namespace TrainEase.Core.Domain.Entities
{
    public class BookTicket : BaseAuditableEntity<int>  // ← أضف
    {
        public decimal Price { get; set; }
        public DateTime CreatedOn { get; set; }

        public int PassengerId { get; set; }
        public int TicketId { get; set; }

        public virtual Passenger Passenger { get; set; } = null!;
        public virtual Ticket Ticket { get; set; } = null!;
    }
}