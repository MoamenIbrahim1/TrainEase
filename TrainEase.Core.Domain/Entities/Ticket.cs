using TrainEase.Core.Domain.Common;

namespace TrainEase.Core.Domain.Entities
{
    public class Ticket : BaseEntity<int>
    {
       
        public decimal Price { get; set; }
        public required string TicketType { get; set; } 
        public int ScheduleId {  get; set; }


        public int BookId { get; set; }
        public int PassengerId { get; set; }

        public virtual Booking Booking { get; set; } = default!;
        public virtual Passenger Passenger { get; set; } = default!;
        public virtual Schedule Schedule { get; set; } = default!;


    }
}