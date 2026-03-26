using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Domain.Entities;
using TrainEase.Infrastructure.Presistance._Data.Configurations.Base;

namespace TrainEase.Infrastructure.Presistance._Data.Configurations.Enities
{
    public class ScheduleConfiguration : BaseEntityConfiguration<Schedule, int>
    {
        public override void Configure(EntityTypeBuilder<Schedule> builder)
        {
            base.Configure(builder);


            builder.HasOne(s => s.SourceStation)
                   .WithMany()
                   .HasForeignKey(s => s.SourceStationId)
                   .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(s => s.DestinationStation)
                   .WithMany()
                   .HasForeignKey(s => s.DestinationStationId)
                   .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(s => s.Train)
                   .WithMany(t => t.Schedules)
                   .HasForeignKey(s => s.TrainId);

            builder.ToTable("Schedules");
        }
    }
}