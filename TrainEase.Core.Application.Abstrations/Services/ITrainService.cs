using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Application.Abstrations.Model.Train;

namespace TrainEase.Core.Application.Abstrations.Services
{
    public interface ITrainService
    {
     public Task<IEnumerable<TrainDto>> GetTrainsAsync();
     public Task<bool> AddTrainAsync(AddTrainDto addTrainDto);
     public Task<TrainDto> EditTrainAsync(int id, EditTrainDto editTrainDto);
     public Task<bool> DeleteTrainAsync(int trainId);
        public Task<IEnumerable<TrainClassDto>> GetTrainClassesAsync(int trainId);


    }
}
