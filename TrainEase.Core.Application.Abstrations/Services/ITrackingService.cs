using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Application.Abstrations.Model.Tracking;

namespace TrainEase.Core.Application.Abstrations.Services
{
    public interface ITrackingService
    {
        Task<TrainTrackingDto> GetTrainTrackingAsync(int scheduleId);
    }
}
