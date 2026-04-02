using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainEase.APIs.Controllers.Controllers.Base;
using TrainEase.Core.Application.Abstrations.Services;
 
namespace TrainEase.APIs.Controllers.Controllers.Journey
{ 
    public class JourneyController(IServiceManager _serviceManager) : BaseApiController
    {
        //[Authorize(Roles = "Passenger")]
        [HttpGet("search")]
        public async Task<IActionResult> SearchJourneys(
         [FromQuery] int fromStationId,
         [FromQuery] int toStationId,
         [FromQuery] DateTime date)
        {
            var result = await _serviceManager.JourneyService
                .SearchJourneysAsync(fromStationId, toStationId, date);

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