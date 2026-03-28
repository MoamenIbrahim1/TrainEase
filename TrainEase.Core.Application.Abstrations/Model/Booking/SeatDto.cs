namespace TrainEase.Core.Application.Abstrations.Model.Booking
{
    public class SeatDto
    {
        public int Id { get; set; }
        public string SeatNumber { get; set; } = null!;
        public string CarriageNumber { get; set; } = null!;
        public string ClassType { get; set; } = null!;
        public bool IsBooked { get; set; }
    }
}