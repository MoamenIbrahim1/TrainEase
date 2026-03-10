using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Application.Abstrations.Model.Journey;

namespace TrainEase.Core.Application.Abstrations.Services
{
    public interface IJourneyService
    {
      Task<IEnumerable<JourneySearchResultDto>> SearchJourneysAsync(
      int fromStationId,
        int toStationId,
        DateTime date);
      Task<IEnumerable<JourneySearchResultDto>> GetAllJourneysAsync();
    }
}
