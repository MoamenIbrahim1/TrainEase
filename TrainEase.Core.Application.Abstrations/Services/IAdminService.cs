using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Application.Abstrations.Model.StationMaster;

namespace TrainEase.Core.Application.Abstrations.Services
{
    public interface IAdminService
    {
     public Task<StationMasterDto> AssignStationMasterAsync(CreateStationMasterDto stationMaster);
    }
}
