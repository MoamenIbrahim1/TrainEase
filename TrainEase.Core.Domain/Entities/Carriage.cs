using System;
using System.Collections.Generic;
using System.Text;

namespace TrainEase.Core.Domain.Entities
{
    public class Carriage
    {
        public int CarriageId { get; set; }
        public int TrainId {  get; set; }
        public int SeatCount { get; set; }
        public virtual ICollection<Seat> Seats { get; set; }
        public virtual Train Train { get; set; } 
    }
}
