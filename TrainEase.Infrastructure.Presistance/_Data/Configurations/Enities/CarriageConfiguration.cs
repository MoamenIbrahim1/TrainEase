using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Domain.Entities;

namespace TrainEase.Infrastructure.Presistance._Data.Configurations.Enities
{
    public class CarriageConfiguration : IEntityTypeConfiguration<Carriage>
    {
        public void Configure(EntityTypeBuilder<Carriage> builder)
        {
            builder.Property(c => c.CarriageNumber).HasMaxLength(20);
            builder.Property(c => c.ClassType).HasMaxLength(50);

            builder.HasOne(c => c.Train)
                   .WithMany(t => t.Carriages)
                   .HasForeignKey(c => c.TrainId)
                   .OnDelete(DeleteBehavior.Cascade); // لو مسحنا القطار تمسح عرباته
        }
    }
}
