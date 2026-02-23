using LinkDev.Talabat.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TrainEase.Core.Domain.Entities;

namespace TrainEase.Infrastructure.Presistance._Data
{
    public class TrainEaseContext : DbContext
    {

        public TrainEaseContext(DbContextOptions options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssemblyInformation).Assembly);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.LogTo(log => Debug.WriteLine(log), LogLevel.Information).
                EnableSensitiveDataLogging(true); // Enable sensitive data logging for debugging purposes

        }
        public DbSet<Train> Trains { get; set; }
        public DbSet<Carriage> Carriages { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<RouteStation> RouteStations { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Refund> Refunds { get; set; }
        public DbSet<Notification> Notifications { get; set; }
    }
}
