using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainEase.Core.Domain.Common
{
    public abstract class BaseAuditableEntity<TKey> : BaseEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
