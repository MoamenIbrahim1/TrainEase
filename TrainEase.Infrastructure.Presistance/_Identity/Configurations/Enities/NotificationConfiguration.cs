using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainEase.Core.Domain.Entities;
using TrainEase.Infrastructure.Presistance._Data.Configurations.Base;

namespace TrainEase.Infrastructure.Presistance._Data.Configurations.Enities
{
    public class NotificationConfiguration : BaseEntityConfiguration<Notification, int>
    {
        public override void Configure(EntityTypeBuilder<Notification> builder)
        {

            base.Configure(builder);

            builder.Property(n => n.Message).IsRequired().HasMaxLength(500);

            builder.HasOne(n => n.User)
        .WithMany(u => u.Notifications)
        .HasForeignKey(n => n.UserId)
        .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(n => n.SentByUser)
                   .WithMany()
                   .HasForeignKey(n => n.SentByUserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}