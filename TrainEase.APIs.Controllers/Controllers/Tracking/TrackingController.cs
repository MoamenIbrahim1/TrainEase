using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainEase.Core.Application.Abstrations.Services;

namespace TrainEase.APIs.Controllers.Tracking
{

    [ApiController]
    [Route("api/tracking")]
    public class TrackingController(ITrackingService _trackingService) : ControllerBase
    {
        [HttpGet("{scheduleId}")]
        [Authorize]
        public async Task<IActionResult> GetTrainTracking(int scheduleId)
        {
            var result = await _trackingService.GetTrainTrackingAsync(scheduleId);
            return Ok(result);
        }
    }
}