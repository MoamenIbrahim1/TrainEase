using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainEase.Core.Application.Abstrations.Model.Tracking
{
    public class TrainTrackingDto
    {
        public int ScheduleId { get; set; }
        public string TrainNumber { get; set; }
        public string CurrentSegment { get; set; }  // "Cairo → Tanta"
        public string FromStationName { get; set; }     // ✅ جديد
        public string ToStationName { get; set; }       // ✅ جديد
        public double CurrentLatitude { get; set; }
        public double CurrentLongitude { get; set; }
        public double ProgressPercent { get; set; }
        public int TotalDelayMinutes { get; set; }
        public int EstimatedRemainingMinutes { get; set; }
        public string Status { get; set; }
    }
}
