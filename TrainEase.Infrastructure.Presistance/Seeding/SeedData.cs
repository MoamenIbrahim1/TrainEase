using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Domain.Entities;
using TrainEase.Infrastructure.Presistance._Data;

namespace TrainEase.Infrastructure.Presistance.Seeding
{
    public static class SeedData
    {
        public static async Task SeedAsync(AppDbContext context)
        {

            if (!context.Stations.Any())
            {
                var stations = new List<Station>
                {
                    new Station { Name = "Cairo Station", City = "Cairo" },
                    new Station { Name = "Alexandria Station", City = "Alexandria" }
                };
                context.Stations.AddRange(stations);
                await context.SaveChangesAsync();
            }


            if (!context.Trains.Any())
            {
                var train = new Train
                {
                    TrainName = "Express 901",
                    TrainNumber = "TR-100",
                    Type = "Speed Train"
                };
                context.Trains.Add(train);
                await context.SaveChangesAsync();

                var carriage = new Carriage
                {
                    TrainId = train.Id,
                    CarriageNumber = "C1",
                    ClassType = "First Class"
                };
                context.Carriages.Add(carriage);
                await context.SaveChangesAsync();


                context.Seats.AddRange(
                    new Seat { CarriageId = carriage.Id, SeatNumber = "A1", IsAvailable = true },
                    new Seat { CarriageId = carriage.Id, SeatNumber = "A2", IsAvailable = true }
                );
                await context.SaveChangesAsync();
            }


            if (!context.Schedules.Any())
            {
                var route = new Route { RouteName = "Cairo-Alex Direct", Sequence = 1 };
                context.Routes.Add(route);
                await context.SaveChangesAsync();

                var stationSource = context.Stations.First(s => s.City == "Cairo");
                var stationDest = context.Stations.First(s => s.City == "Alexandria");


                var train = context.Trains.First();

                context.Schedules.Add(new Schedule
                {
                    TrainId = train.Id,
                    RouteId = route.Id,
                    SourceStationId = stationSource.Id,
                    DestinationStationId = stationDest.Id,
                    DepartureTime = DateTime.Now.AddHours(2),
                    ArrivalTime = DateTime.Now.AddHours(5),
                    Price = 150.00m
                });
                await context.SaveChangesAsync();
            }
        }
    }
}
