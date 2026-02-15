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
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.FullName).HasMaxLength(150).IsRequired();
            builder.Property(u => u.Role).IsRequired();

            // One-to-Many: User -> Bookings
            builder.HasMany(u => u.Bookings)
                   .WithOne(b => b.User)
                   .HasForeignKey(b => b.UserId)
                   .OnDelete(DeleteBehavior.Restrict); // منع مسح المستخدم لو عنده حجوزات
        }
    }
}
