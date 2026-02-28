using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Domain.Common;

namespace TrainEase.Core.Domain.Entities
{
    public class Train : BaseAuditableEntity<int>
    {
        public string? TrainNumber { get; set; }
        public string? TrainName { get; set; }
        public string? Type { get; set; }
        public virtual ICollection<Carriage> Carriages { get; set; } = new HashSet<Carriage>();
        public virtual ICollection<Schedule> Schedules { get; set; } = new HashSet<Schedule>();
    }
}
