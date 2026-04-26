using TrainEase.Core.Domain.Common;

namespace TrainEase.Core.Domain.Entities
{
    public class StationMaster : BaseAuditableEntity<int>
    {
        public virtual ICollection<Notification> Notfications { get; set; }
        public int StationID { get; set; }
        public virtual Station Station { get; set; }
    }
}
