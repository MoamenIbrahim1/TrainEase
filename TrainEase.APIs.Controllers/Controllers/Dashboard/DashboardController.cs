using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.APIs.Controllers.Controllers.Base;
using TrainEase.Core.Application.Abstrations.Services;

namespace TrainEase.APIs.Controllers.Controllers.Dashboard
{
    public class DashboardController(IServiceManager _serviceManager) : BaseApiController
    {
        [HttpGet("GetTotalTrains")]
        public async Task<IActionResult> GetTotalTrains()
        {
            var totalTrains = await _serviceManager.DashboardService.GetTotalTrainsAsync();
            return Ok(totalTrains);
        }
        [HttpGet("GetTotalActivePassengers")]
        public async Task<IActionResult> GetTotalActivePassengers()
        {
            var totalActivePassengers = await _serviceManager.DashboardService.GetTotalActivePassengersAsync();
            return Ok(totalActivePassengers);
        }
        [HttpGet("GetTodaysBookings")]
        public async Task<IActionResult> GetTodaysBookings()
        {
            var todaysBookings = await _serviceManager.DashboardService.GetTodaysBookingsAsync();
            return Ok(todaysBookings);
        }
        [HttpGet("GetAllTrainSeatStats")]
        public async Task<IActionResult> GetAllTrainSeatStats()
        {
            var trainSeatStats = await _serviceManager.DashboardService.GetAllTrainSeatStatsAsync();
            return Ok(trainSeatStats);
        }
    }
}