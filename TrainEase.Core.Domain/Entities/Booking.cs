using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Domain.Common;
using TrainEase.Core.Domain.Entities.Enums;

namespace TrainEase.Core.Domain.Entities
{
    public class Booking : BaseAuditableEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public BookingStatus Status { get; set; }

        public ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();
        public Payment? Payment { get; set; } 
        public Invoice? Invoice { get; set; }
        public Refund? Refund { get; set; }
    }
}
