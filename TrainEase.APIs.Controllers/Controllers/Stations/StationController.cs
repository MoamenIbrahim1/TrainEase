using Microsoft.AspNetCore.Mvc;
using TrainEase.APIs.Controllers.Controllers.Base;
using TrainEase.Core.Application.Abstrations.Services;

namespace TrainEase.APIs.Controllers.Controllers.Stations
{
    public class StationController(IServiceManager _serviceManager) : BaseApiController
    {
        [HttpGet("[Action]")]
        public async Task<IActionResult> GetAllStations()
        {
            var stations = await _serviceManager.StationService.GetAllStationsAsync();
            return Ok(stations);
        }
    }
}