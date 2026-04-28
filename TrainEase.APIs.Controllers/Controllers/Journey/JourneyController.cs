using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainEase.APIs.Controllers.Controllers.Base;
using TrainEase.Core.Application.Abstrations.Model.Journey;
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

        [HttpPost("CreateJourney")]
        public async Task<IActionResult> CreateJourney([FromBody] JourneyCreateDto journeyCreateDto)
        {
            var result = await _serviceManager.JourneyService.CreateJourneyAsync(journeyCreateDto);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteJourney/{journeyId}")]
        public async Task<IActionResult> DeleteJourney(int journeyId)
        {
            var result = await _serviceManager.JourneyService.DeleteJourneyAsync(journeyId);
            return Ok(result);
        }
    }
}