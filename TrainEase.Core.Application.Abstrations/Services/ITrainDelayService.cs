using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Application.Abstrations.Model.Delays;
using TrainEase.Core.Application.Abstrations.Model.Stations;

namespace TrainEase.Core.Application.Abstrations.Services
{
        public interface ITrainDelayService
        {
            Task<TrainDelayResultDto> ReportDelayAsync(ReportDelayDto dto, string stationMasterUserId);
            Task<IEnumerable<TrainDelayResultDto>> GetAllDelaysAsync();
            Task<IEnumerable<TrainDelayResultDto>> GetDelaysByStationAsync(int stationId);
            Task<int> GetStationIdByUserAsync(string userId);
        Task<IEnumerable<StationScheduleDto>> GetSchedulesByStationAsync(int stationId);
    }
    }

