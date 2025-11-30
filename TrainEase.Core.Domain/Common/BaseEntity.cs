using System;
using System.Collections.Generic;
using System.Text;

namespace TrainEase.Core.Domain.Common
{
    public class BaseEntity<Tkey> where Tkey : IEquatable<Tkey>
    {
        public required Tkey Id { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public required string CreatedBy { get; set; }

        public required string LastModifiedBy { get; set; }
        public DateTime LastModifiedOn { get; set; } = DateTime.UtcNow;

    }
}
