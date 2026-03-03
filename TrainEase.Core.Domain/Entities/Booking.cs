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
    public class Booking : BaseAuditableEntity<int>{
        public string? UserId { get; set; }
        public virtual User User { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public BookingStatus Status { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();
        public virtual Payment? Payment { get; set; } 
        public virtual Invoice? Invoice { get; set; }
        public virtual Refund? Refund { get; set; }
    }
}
