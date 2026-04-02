using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainEase.Core.Application.Abstrations.Model.Booking
{
    public class BookingResultDto
    {
        public int BookingId { get; set; }
        public string Status { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public DateTime BookingDate { get; set; }
        public List<TicketResultDto> Tickets { get; set; } = new();
    }

}
