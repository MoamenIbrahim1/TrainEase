namespace TrainEase.Core.Domain.Entities
{
    public class Notification
    {
        public int NotficationId { get; set; }
      
        public int TrainId {  get; set; }
        public int StationMasterId {  get; set; }
        public int PassengerId {  get; set; }
        public DateTime CreatedOn { get; set; }
        public virtual Train Train { get; set; }
        public virtual StationMaster StationMaster { get; set; }
        public virtual Passenger Passenger { get; set; } = null!;
    }
}