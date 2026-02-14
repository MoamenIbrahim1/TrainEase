using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TrainEase.Core.Domain.Common;

namespace TrainEase.Infrastructure.Presistance._Data.Configurations.Base
{
    public class BaseEntityConfiguration<TEntity , TKey> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity<> 
    {
    }
}
