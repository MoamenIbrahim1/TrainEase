using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainEase.Core.Domain.Common;

namespace TrainEase.Infrastructure.Presistance._Data.Configurations.Base
{
    public class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
     where TEntity : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            // Primary Key
            builder.HasKey(e => e.Id);

            // Audit Properties
            builder.Property(e => e.CreatedAt)
                   .HasDefaultValueSql("GETDATE()") // أو DateTime.Now حسب قاعدة البيانات
                   .IsRequired(false);

            builder.Property(e => e.UpdatedAt)
                   .IsRequired(false);

        }
    }
}
