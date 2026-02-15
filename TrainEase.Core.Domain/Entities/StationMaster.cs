using System;
using System.Collections.Generic;
using System.Text;

namespace TrainEase.Core.Domain.Entities
{
    public class StationMaster : User<int>
    {       
        public virtual ICollection<Notification> Notfications {  get; set; }
        public int StationID { get; set; }
        public Station Station { get; set; }
    }
}
