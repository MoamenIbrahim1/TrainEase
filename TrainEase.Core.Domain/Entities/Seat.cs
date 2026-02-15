using TrainEase.Core.Domain.Common;

namespace TrainEase.Core.Domain.Entities
{
    public class Seat : BaseAuditableEntity
    {
        public int CarriageId { get; set; }
        public Carriage Carriage { get; set; } = null!;
        public string? SeatNumber { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}