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

        [Required]
        [MinLength(1, ErrorMessage = "يجب إضافة راكب واحد على الأقل")]
        public List<PassengerBookingDto> Passengers { get; set; } = new List<PassengerBookingDto>();
    }
}
