using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainEase.Core.Application.Abstrations.Model.Offers
{
    public class OfferResultDto
    {
        public int OfferId { get; set; }
        public int ScheduleId { get; set; }
        public string TrainName { get; set; } = null!;
        public string TrainNumber { get; set; } = null!;
        public decimal DiscountPercentage { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsActive { get; set; }
    }
}
