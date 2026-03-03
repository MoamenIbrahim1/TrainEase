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
    public class BookingConfiguration : BaseEntityConfiguration<Booking,int>
    {
        public override void Configure(EntityTypeBuilder<Booking> builder)
        {
            base.Configure(builder);
            builder.Property(b => b.TotalAmount).HasColumnType("decimal(18,2)");

            // علاقة 1-to-1 مع الـ Invoice
            builder.HasOne(b => b.Invoice)
                   .WithOne(i => i.Booking)
                   .HasForeignKey<Invoice>(i => i.BookingId);

            // علاقة 1-to-1 مع الـ Payment
            builder.HasOne(b => b.Payment)
                   .WithOne(p => p.Booking)
                   .HasForeignKey<Payment>(p => p.BookingId);

            // علاقة 1-to-1 مع الـ Refund
            builder.HasOne(b => b.Refund)
                   .WithOne(r => r.Booking)
                   .HasForeignKey<Refund>(r => r.BookingId);
        }
    }
}
