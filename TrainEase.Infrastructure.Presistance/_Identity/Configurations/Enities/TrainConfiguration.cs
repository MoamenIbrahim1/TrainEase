using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainEase.Core.Domain.Entities;

namespace TrainEase.Infrastructure.Presistance._Data.Configurations.Enities
{
    public class TrainConfiq : IEntityTypeConfiguration<Train>
    {
        public void Configure(EntityTypeBuilder<Train> builder)
        {
            builder.ToTable("Trains");
        }
    }
}