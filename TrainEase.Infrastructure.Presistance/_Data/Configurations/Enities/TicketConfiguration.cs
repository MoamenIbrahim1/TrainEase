using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Domain.Entities;
using TrainEase.Infrastructure.Presistance._Data.Configurations.Base;

namespace TrainEase.Infrastructure.Presistance._Data.Configurations.Enities
{
    public class TicketConfiguration : BaseEntityConfiguration<Ticket,int>
    {
        public override void Configure(EntityTypeBuilder<Ticket> builder)
        {
            base.Configure(builder);

            builder.Property(t => t.Price).HasColumnType("decimal(18,2)");

            builder.HasOne(t => t.Seat)
                   .WithMany() // المقعد الواحد ممكن تطلعه تذاكر في رحلات مختلفة (مستقبلاً)
                   .HasForeignKey(t => t.SeatId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Booking)
                   .WithMany(b => b.Tickets)
                   .HasForeignKey(t => t.BookingId);
        }
    }
}
