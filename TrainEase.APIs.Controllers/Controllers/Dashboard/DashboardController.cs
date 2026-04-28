using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.APIs.Controllers.Controllers.Base;
using TrainEase.Core.Application.Abstrations.Services;
using TrainEase.Core.Application.Services;

namespace TrainEase.APIs.Controllers.Controllers.Dashboard
{
    [Authorize(Roles = "Admin")]
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
        [HttpGet("GetAllDelays")]
        public async Task<IActionResult> GetAllDelays()
        {
            var result = await _serviceManager.TrainDelayService.GetAllDelaysAsync();
            return Ok(result);
        }
        [HttpGet("AllBookings")]
        public async Task<IActionResult> GetAllBookings()
        {
            var result = await _serviceManager.BookingService
                .GetAllBookingsAsync();
            return Ok(result);
        }
        [HttpGet("AllJourneys")]
        public async Task<IActionResult> GetAllJourneys()
        {
            var result = await _serviceManager.JourneyService.GetAllJourneysAsync();
            return Ok(result);
        }
    }
}