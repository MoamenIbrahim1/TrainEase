using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Application.Abstrations.Model.Station;
using TrainEase.Core.Application.Abstrations.Services;
using TrainEase.Core.Domain.Contracts;
using TrainEase.Core.Domain.Entities;

namespace TrainEase.Core.Application.Services.Stations
{
    internal class StationService(IUnitOfWork _unitOfWork,IMapper _mapper) : IStationService
    {
        public async Task<IEnumerable<StationToReturnDto>> GetAllStationsAsync()
        {
            var stations =  _unitOfWork.GetRepository<Station, int>();
            var allStations = await stations.GetAllAsync();
            var result = _mapper.Map<IEnumerable<StationToReturnDto>>(allStations);
            return result;
        }
    }
}
