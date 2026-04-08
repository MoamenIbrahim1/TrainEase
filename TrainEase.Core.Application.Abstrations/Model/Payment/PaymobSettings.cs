using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainEase.Core.Application.Abstrations.Model.Payment
{
    public class PaymobSettings
    {
        public string ApiKey { get; set; } = null!;
        public int IntegrationId { get; set; }
        public string IframeId { get; set; } = null!;
        public string HmacSecret { get; set; } = null!;
    }
}
