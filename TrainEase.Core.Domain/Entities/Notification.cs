using TrainEase.Core.Domain.Common;

namespace TrainEase.Core.Domain.Entities
{
    public class Notification : BaseAuditableEntity<int>
    {
        public string UserId { get; set; } = null!;
        public virtual User User { get; set; } = null!;

        public string Message { get; set; } = null!;
        public DateTime SentAt { get; set; }

        public string? SentByUserId { get; set; }
        public virtual User? SentByUser { get; set; }
    }
}