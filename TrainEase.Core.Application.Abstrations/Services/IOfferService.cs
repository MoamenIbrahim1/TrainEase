using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Application.Abstrations.Model.Offers;

namespace TrainEase.Core.Application.Abstrations.Services
{
    public interface IOfferService
    {
        Task<OfferResultDto> CreateOfferAsync(CreateOfferDto dto, string adminUserId);
        Task<IEnumerable<OfferResultDto>> GetActiveOffersAsync();
        Task<OfferResultDto?> GetActiveOfferByScheduleAsync(int scheduleId);
    }
}
