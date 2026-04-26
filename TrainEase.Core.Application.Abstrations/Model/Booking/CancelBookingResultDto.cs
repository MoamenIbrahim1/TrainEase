namespace TrainEase.Core.Application.Abstrations.Model.Booking
{
    public class CancelBookingResultDto
    {
        public int BookingId { get; set; }
        public string BookingStatus { get; set; } = null!;
        public decimal OriginalAmount { get; set; }
        public decimal RefundAmount { get; set; }
        public decimal PenaltyAmount { get; set; }
        public DateTime CancellationDate { get; set; }
        public string Message { get; set; } = null!;
    }
}