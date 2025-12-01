namespace TrainEase.Core.Domain.Entities
{
    public class Seat
    {
        public int SeatId { get; set; }
        public int CarriageId {  get; set; }
       
        public virtual Carriage Carriage{ get; set; }
    }
}