using TrainEase.Core.Domain.Common;

namespace TrainEase.Core.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!; // المستلم

        public string Message { get; set; } = null!;
        public DateTime SentAt { get; set; }

        public int? SentByUserId { get; set; }  // StationMaster 
        public User? SentByUser { get; set; }
    }
}