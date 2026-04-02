using Microsoft.AspNetCore.Mvc;
using TrainEase.APIs.Controllers.Controllers.Base;
using TrainEase.Core.Application.Abstrations.Model.StationMaster;
using TrainEase.Core.Application.Abstrations.Services;

namespace TrainEase.APIs.Controllers.Controllers.Admin
{
    public class AdminController(IServiceManager _serviceManager) : BaseApiController
    {
        [HttpPost("[action]")]
        public async Task<ActionResult<StationMasterDto>> AssignStationMaster(CreateStationMasterDto stationMaster)
        {
           var result = await _serviceManager.AdminService.AssignStationMasterAsync(stationMaster);
           return Ok(result);
        }
        [HttpDelete("[action]/{stationMasterId}")]
        public async Task<ActionResult<bool>> RemoveStationMaster(string stationMasterId)
        {
            var result = await _serviceManager.AdminService.RemoveStationMasterAsync(stationMasterId);
            return Ok("Station master removed successfully.");
        }
    }
}
