using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainEase.Core.Application.Abstrations.Model.Delays
{
   public class ReportDelayDto
    {
        public int ScheduleId { get; set; }
        public int? DelayMinutes { get; set; }
    }
}
