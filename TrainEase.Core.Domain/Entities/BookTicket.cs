using System;
using System.Collections.Generic;
using System.Text;

namespace TrainEase.Core.Domain.Entities
{
    public class BookTicket
    {
        public int Id { get; set; } 
        public decimal Price { get; set; }
        public DateTime CreatedOn { get; set; }
        //forign keys
        public int PassengerId { get; set; }
        public int TicketId { get; set; }

     //Navigation Properitrs
        public virtual Passenger Passenger { get; set; } 
        public virtual Ticket Ticket { get; set; }
    }
}
