using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainEase.Core.Application.Abstrations.Model.Booking
{
    public class CreateBookingDto
    {
        [Required]
        public int ScheduleId { get; set; }
        public int UserSeatId { get; set; }
        public List<PassengerBookingDto> AdditionalPassengers { get; set; } = new();
    }
}
