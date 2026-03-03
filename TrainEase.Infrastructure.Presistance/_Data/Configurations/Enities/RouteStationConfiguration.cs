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
    public class RouteStationConfiguration : BaseEntityConfiguration<RouteStation,int>
    {
        public override void Configure(EntityTypeBuilder<RouteStation> builder)
        {
            base.Configure(builder);

            builder.HasKey(rs => new { rs.RouteId, rs.StationId }); // Composite Key لو مش مستخدم BaseEntity ID

            builder.HasOne(rs => rs.Route)
                   .WithMany(r => r.RouteStations)
                   .HasForeignKey(rs => rs.RouteId);

            builder.HasOne(rs => rs.Station)
                   .WithMany(s => s.RouteStations)
                   .HasForeignKey(rs => rs.StationId);
        }
    }
}
