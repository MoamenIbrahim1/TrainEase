using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainEase.Core.Application.Abstrations.Model.Booking
{
    public class PassengerBookingDto
    {
        [Required(ErrorMessage = "الاسم بالكامل مطلوب")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "الرقم القومي مطلوب")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "الرقم القومي يجب أن يكون 14 رقم")]
        public string NationalId { get; set; } = null!;

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [Phone]
        public string PhoneNumber { get; set; } = null!;

        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
        public string? Email { get; set; }
        [Required]
        public int SeatId { get; set; }
    }
}
