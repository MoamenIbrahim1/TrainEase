using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrainEase.APIs.Controllers.Controllers.Base;
using TrainEase.Core.Application.Abstrations.Model.Delays;
using TrainEase.Core.Application.Abstrations.Services;

namespace TrainEase.APIs.Controllers.Controllers.Delays
{
    public class TrainDelayController(IServiceManager serviceManager) : BaseApiController
    {
        // StationMaster يبلغ عن تأخير
        [Authorize(Roles = "StationMaster")]
        [HttpPost("report")]
        public async Task<IActionResult> ReportDelay([FromBody] ReportDelayDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await serviceManager.TrainDelayService.ReportDelayAsync(dto, userId);
            return Ok(result);
        }

       

        // StationMaster يشوف تأخيرات محطته
        [Authorize(Roles = "StationMaster")]
        [HttpGet("DelaysInMyStation")]
        public async Task<IActionResult> GetMyStationDelays()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var stationId = await serviceManager.TrainDelayService.GetStationIdByUserAsync(userId);
            var result = await serviceManager.TrainDelayService.GetDelaysByStationAsync(stationId);
            return Ok(result);
        }

        // StationMaster يشوف القطارات في محطته عشان يختار منها
        [Authorize(Roles = "StationMaster")]
        [HttpGet("my-station/schedules")]
        public async Task<IActionResult> GetMyStationSchedules()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var stationId = await serviceManager.TrainDelayService.GetStationIdByUserAsync(userId);
            var result = await serviceManager.TrainDelayService.GetSchedulesByStationAsync(stationId);
            return Ok(result);
        }
    }
}