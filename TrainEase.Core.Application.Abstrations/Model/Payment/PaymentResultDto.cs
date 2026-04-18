using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainEase.Core.Application.Abstrations.Model.Payment
{
    public class PaymentResultDto
    {
        public string IframeUrl { get; set; } = null!;
        public string PaymentKey { get; set; } = null!;
    }
}
