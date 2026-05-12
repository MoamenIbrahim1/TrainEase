using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Domain.Common;
using TrainEase.Core.Domain.Entities.Enums;

namespace TrainEase.Core.Domain.Entities
{
    public class TrainTracking : BaseEntity<int>
    {
        public int ScheduleId { get; set; }
        public virtual Schedule Schedule { get; set; }

        // المحطة اللي القطار اتحرك منها
        public int DepartedFromStationId { get; set; }
        public virtual Station DepartedFromStation { get; set; }

        // المحطة اللي القطار ماشي ليها
        public int HeadingToStationId { get; set; }
        public virtual Station HeadingToStation { get; set; }
        // وقت ما القطار اتحرك من المحطة الأخيرة (بيتحسب من الـ Schedule + أي تأخير)
        public DateTime ActualDepartureTime { get; set; }

        // وقت الوصول المتوقع للمحطة الجاية (بيتحسب من الـ Schedule + أي تأخير)
        public DateTime ExpectedArrivalTime { get; set; }

        // المسافة الكاملة بين المحطتين بالمتر
        public double TotalDistanceMeters { get; set; }

        // التأخير المُضاف بالدقائق (من TrainDelay)
        public int DelayMinutes { get; set; }

    }
}
