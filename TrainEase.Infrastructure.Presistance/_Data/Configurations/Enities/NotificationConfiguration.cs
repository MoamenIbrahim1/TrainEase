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
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.Property(n => n.Message).IsRequired().HasMaxLength(500);

            // المستلم
            builder.HasOne(n => n.User)
                   .WithMany(u => u.Notifications)
                   .HasForeignKey(n => n.UserId);

            // الراسل (Station Master)
            builder.HasOne(n => n.SentByUser)
                   .WithMany()
                   .HasForeignKey(n => n.SentByUserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
