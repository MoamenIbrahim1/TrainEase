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
    public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
    {
        public void Configure(EntityTypeBuilder<Schedule> builder)
        {
            // ربط محطة القيام
            builder.HasOne(s => s.SourceStation)
                   .WithMany()
                   .HasForeignKey(s => s.SourceStationId)
                   .OnDelete(DeleteBehavior.Restrict); // مهم جداً لمنع الـ Cycle
  //لو موظف مسح محطة "القاهرة" بالغلط، السيستم ما يروحش يمسح "كل الرحلات" اللي رايحة وجاية من القاهرة

            // ربط محطة الوصول
            builder.HasOne(s => s.DestinationStation)
                   .WithMany()
                   .HasForeignKey(s => s.DestinationStationId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Train)
                   .WithMany(t => t.Schedules)
                   .HasForeignKey(s => s.TrainId);
        }
    }
}
