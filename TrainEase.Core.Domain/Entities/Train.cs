namespace TrainEase.Core.Domain.Entities
{
    public class Train
    {
        public int TrainId { get; set; }
        public string TrainType { get; set; }
        public int CarriageCount { get; set; }
        public int SeatsCount { get; set; }
        public int ScheduleId { get; set; }
        public virtual ICollection<Carriage> Carriages { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
    }
}