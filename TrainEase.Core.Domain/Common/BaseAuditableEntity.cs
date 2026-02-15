using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainEase.Core.Domain.Common
{
    internal abstract class BaseAuditableEntity<TKey>:BaseAuditableEntity<TKey>
        where TKey:IEquatable<TKey>
    {
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
