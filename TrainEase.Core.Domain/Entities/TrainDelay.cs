using TrainEase.Core.Domain.Common;

namespace TrainEase.Core.Domain.Entities
{
    public class TrainDelay : BaseAuditableEntity<int>
    {
        public int ScheduleId { get; set; }
        public virtual Schedule Schedule { get; set; } = null!;

        public string ReportedByUserId { get; set; } = null!;
        public virtual User ReportedByUser { get; set; } = null!;

        public int? DelayMinutes { get; set; }
        public string? Reason { get; set; }
        public DateTime ReportedAt { get; set; }

        // عدد الركاب اللي اتبعتلهم SMS
        public int NotifiedPassengersCount { get; set; }
    }
}