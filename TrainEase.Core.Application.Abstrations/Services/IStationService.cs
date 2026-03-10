using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Application.Abstrations.Model.Station;

namespace TrainEase.Core.Application.Abstrations.Services
{
    public interface IStationService
    {
        Task<IEnumerable<StationToReturnDto>> GetAllStationsAsync();
    }
}
