using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.APIs.Controllers.Controllers.Base;
using TrainEase.Core.Application.Abstrations.Model.Train;
using TrainEase.Core.Application.Abstrations.Services;

namespace TrainEase.APIs.Controllers.Controllers.Train
{
    public class TrainController(IServiceManager _serviceManager) : BaseApiController
    {
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<TrainDto>>> GetTrains()
        {
            var result = await _serviceManager.TrainService.GetTrainsAsync();
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> AddTrain(AddTrainDto addTrainDto)
        {
            var result = await _serviceManager.TrainService.AddTrainAsync(addTrainDto);
            if (!result)
                return BadRequest("Failed to add train");
            return Ok("Train added successfully");
        }

        [HttpDelete("[action]")]
        public async Task<ActionResult> DeleteTrain(int trainId)
        {
            var result = await _serviceManager.TrainService.DeleteTrainAsync(trainId);
            if (!result)
                return BadRequest("Failed to delete train");
            return Ok("Train deleted successfully");
        }

        [HttpPut("[action]/{id}")]
        public async Task<ActionResult<TrainDto>> EditTrain(int id, [FromBody] EditTrainDto editTrainDto)
        {
            var result = await _serviceManager.TrainService.EditTrainAsync(id ,editTrainDto);
            return Ok(result);
        }
    }
}