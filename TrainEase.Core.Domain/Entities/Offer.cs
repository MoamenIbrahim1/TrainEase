using TrainEase.Core.Domain.Common;

namespace TrainEase.Core.Domain.Entities
{
    public class Offer : BaseAuditableEntity<int>
    {
        public int ScheduleId { get; set; }
        public virtual Schedule Schedule { get; set; } = null!;

        public string CreatedByUserId { get; set; } = null!;
        public virtual User CreatedByUser { get; set; } = null!;

        public decimal DiscountPercentage { get; set; }

        public DateTime ExpiresAt { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
