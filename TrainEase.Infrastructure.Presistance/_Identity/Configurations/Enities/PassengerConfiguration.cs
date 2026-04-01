using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainEase.Core.Domain.Entities;

namespace TrainEase.Infrastructure.Presistance._Data.Configurations.Enities
{
    internal class PassengerConfiq : IEntityTypeConfiguration<Passenger>
    {
        public void Configure(EntityTypeBuilder<Passenger> builder)
        {
            builder.ToTable("Passengers");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Email).IsRequired().HasMaxLength(150);
            builder.Property(p => p.PhoneNumber).IsRequired().HasMaxLength(15);
            builder.Property(p => p.NationalId).IsRequired().HasMaxLength(14);
            builder.Property(p => p.Age).IsRequired();


            
        }
    }
}