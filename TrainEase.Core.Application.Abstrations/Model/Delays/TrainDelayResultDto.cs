namespace TrainEase.Core.Application.Abstrations.Model.Delays
{
    public class TrainDelayResultDto
    {
        public int DelayId { get; set; }
        public int ScheduleId { get; set; }
        public string TrainNumber { get; set; } = null!;
        public string TrainName { get; set; } = null!;
        public int? DelayMinutes { get; set; }
        public DateTime ReportedAt { get; set; }
        public int NotifiedPassengersCount { get; set; }
        public string StationName { get; set; } = null!;
    }
}
