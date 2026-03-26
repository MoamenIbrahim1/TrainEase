using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Domain.Entities;

namespace TrainEase.Core.Domain.Contracts
{
    public interface IAppDbContext
    {
        DbSet<Seat> Seats { get; }
        DbSet<Booking> Bookings { get; }
        DbSet<Passenger> Passengers { get; }
        DbSet<Schedule> Schedules { get; }
        DbSet<Ticket> Tickets { get; }
        DbSet<BookTicket> BookTickets { get; }
        DatabaseFacade Database { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
