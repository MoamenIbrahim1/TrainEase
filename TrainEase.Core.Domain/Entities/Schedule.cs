namespace TrainEase.Core.Domain.Entities
{
    public class Schedule
    {
        public int ScheduleId { get; set; }
        public int SourceStationId { get; set; }
        public int DestinationStationId { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int TrainId {  get; set; }
        public virtual Train Train {  get; set; }
        [ForeignKey("SourceStationId")]
        public Station SourceStation { get; set; }

        [ForeignKey("DestinationStationId")]
        public Station DestinationStation { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }


    }
}