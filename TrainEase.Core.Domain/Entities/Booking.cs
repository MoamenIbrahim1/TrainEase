using System.Diagnostics;
using TrainEase.Core.Domain.Common;

namespace TrainEase.Core.Domain.Entities
{
    public class Booking : BaseEntity<int>
    {
        public int TicketCount { get; set; }
        public DateTime BookingDate { get; set; }
        public decimal TotalPrice { get; set; }

        public int PassengerId { get; set; }
        public int TrainId { get; set; }
        public int ScheduleId { get; set; }

     
        public virtual Passenger Passenger { get; set; } = default!;
        public virtual Train Train { get; set; } = default!;
        public virtual Schedule Schedule { get; set; } = default!;
               
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
    }
}