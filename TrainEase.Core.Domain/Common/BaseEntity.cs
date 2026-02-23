using System;
using System.Collections.Generic;
using System.Text;

namespace TrainEase.Core.Domain.Common
{
    public abstract class BaseEntity<Tkey>
       where Tkey : IEquatable<Tkey>
    {
        public Tkey? Id { get; set; }

    }
}
