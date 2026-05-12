namespace TrainEase.Core.Application.Abstrations.Model.Booking
{
    public class BookingResultDto
    {
        public int ScheduleId { get; set; }
        public int BookingId { get; set; }
        public string Status { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public DateTime BookingDate { get; set; }
        public List<TicketResultDto> Tickets { get; set; } = new();
    }

}
