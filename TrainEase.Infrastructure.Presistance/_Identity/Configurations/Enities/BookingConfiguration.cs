using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainEase.Core.Domain.Entities;
using TrainEase.Infrastructure.Presistance._Data.Configurations.Base;

namespace TrainEase.Infrastructure.Presistance._Data.Configurations.Enities
{
    public class BookingConfiguration : BaseEntityConfiguration<Booking, int>
    {
        public override void Configure(EntityTypeBuilder<Booking> builder)
        {
            base.Configure(builder);

            builder.Property(b => b.TotalAmount).HasColumnType("decimal(18,2)");



            builder.HasOne(b => b.User)
           .WithMany()
           .HasForeignKey(b => b.UserId)
           .IsRequired(false)
           .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(b => b.Invoice)
                   .WithOne(i => i.Booking)
                   .HasForeignKey<Invoice>(i => i.BookingId);


            builder.HasOne(b => b.Payment)
                   .WithOne(p => p.Booking)
                   .HasForeignKey<Payment>(p => p.BookingId);


            builder.HasOne(b => b.Refund)
                   .WithOne(r => r.Booking)
                   .HasForeignKey<Refund>(r => r.BookingId);
        }
    }
}