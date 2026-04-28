using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainEase.Core.Application.Abstrations.Model.Offers
{
    public class CreateOfferDto
    {
        public int ScheduleId { get; set; }
        public decimal DiscountPercentage { get; set; }
    }

}
