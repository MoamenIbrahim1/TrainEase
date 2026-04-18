using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Application.Abstrations.Model.Dashboard;

namespace TrainEase.Core.Application.Abstrations.Services
{
    public interface IDashboardService
    {
            Task<int> GetTotalTrainsAsync();
            Task<int> GetTotalActivePassengersAsync();
            Task<int> GetTodaysBookingsAsync();
        Task<IEnumerable<TrainSeatStatsDto>> GetAllTrainSeatStatsAsync();
    }
}
