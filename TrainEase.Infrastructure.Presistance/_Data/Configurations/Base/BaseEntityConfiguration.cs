using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders; 
using TrainEase.Core.Domain.Common;
namespace TrainEase.Infrastructure.Presistance._Data.Configurations.Base
{
    
    public class BaseEntityConfiguration<TEntity, TKey> : IEntityTypeConfiguration<TEntity>
        where TEntity : BaseAuditableEntity<TKey> 
        where TKey : IEquatable<TKey>
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CreatedAt)
                   .IsRequired(false); 

            builder.Property(x => x.UpdatedAt)
                   .IsRequired(false);

            builder.Property(x => x.IsDeleted)
                   .HasDefaultValue(false);
        }
    }
}