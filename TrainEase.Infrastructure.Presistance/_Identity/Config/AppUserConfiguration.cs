using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Domain.Entities;

namespace TrainEase.Infrastructure.Presistance._Identity.Config
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.FullName).HasMaxLength(150).IsRequired();
                builder.Property(u => u.NationalId)
                       .HasMaxLength(14)
                       .IsFixedLength() // عشان نضمن إنه دايماً 14 رقم
                       .IsRequired();

                builder.Property(u => u.Role).IsRequired();

                // One-to-Many: User -> Bookings
                builder.HasMany(u => u.Bookings)
                       .WithOne(b => b.User)
                       .HasForeignKey(b => b.UserId)
                       .OnDelete(DeleteBehavior.Restrict);

                // إضافة الـ Notifications علاقة (One-to-Many)
                builder.HasMany(u => u.Notifications)
                       .WithOne(n => n.User)
                       .HasForeignKey(n => n.UserId)
                       .OnDelete(DeleteBehavior.Cascade); // لو اليوزر اتمسح تنبيهاته تتمسح
            }
        }
    }

