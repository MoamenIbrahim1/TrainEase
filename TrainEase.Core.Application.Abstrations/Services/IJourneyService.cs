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
        Task<JourneySearchResultDto> CreateJourneyAsync(JourneyCreateDto journeyCreateDto);
        Task<bool> DeleteJourneyAsync(int journeyId);
    }
}
