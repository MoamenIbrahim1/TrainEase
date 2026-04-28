using Microsoft.EntityFrameworkCore;
using TrainEase.Core.Application.Abstrations.Model.Offers;
using TrainEase.Core.Application.Abstrations.Services;
using TrainEase.Core.Application.Exceptions;
using TrainEase.Core.Domain.Contracts;
using TrainEase.Core.Domain.Entities;

namespace TrainEase.Core.Application.Services.Offers
{
    public class OfferService(IUnitOfWork unitOfWork) : IOfferService
    {
       
        public async Task<OfferResultDto> CreateOfferAsync(CreateOfferDto dto, string adminUserId)
        {
            if (dto.DiscountPercentage <= 0 || dto.DiscountPercentage >= 100)
                throw new BadRequestException("Discount percentage must be between 1 and 99.");

            var scheduleRepo = unitOfWork.GetRepository<Schedule, int>();
            var schedule = await scheduleRepo
                .GetQueryable(s => s.Id == dto.ScheduleId && !s.IsDeleted)
                .Include(s => s.Train)
                .FirstOrDefaultAsync();

            if (schedule is null)
                throw new NotFoundException("Schedule", dto.ScheduleId);

            if (schedule.DepartureTime <= DateTime.UtcNow)
                throw new BadRequestException("Cannot create an offer for a past or ongoing journey.");

            var offerRepo = unitOfWork.GetRepository<Offer, int>();
            var existingOffer = await offerRepo
                .GetQueryable(o => o.ScheduleId == dto.ScheduleId && o.IsActive && !o.IsDeleted)
                .FirstOrDefaultAsync();

            if (existingOffer is not null)
            {
                existingOffer.IsActive = false;
                existingOffer.UpdatedAt = DateTime.UtcNow;
                offerRepo.Update(existingOffer);
            }

            var offer = new Offer
            {
                ScheduleId = dto.ScheduleId,
                CreatedByUserId = adminUserId,
                DiscountPercentage = dto.DiscountPercentage,
                ExpiresAt = schedule.DepartureTime, // ينتهي لما الرحلة تبدأ
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await offerRepo.AddAsync(offer);
            await unitOfWork.CompeleteAsync();

            return new OfferResultDto
            {
                OfferId = offer.Id,
                ScheduleId = dto.ScheduleId,
                TrainName = schedule.Train.TrainName!,
                TrainNumber = schedule.Train.TrainNumber!,
                DiscountPercentage = offer.DiscountPercentage,
                ExpiresAt = offer.ExpiresAt,
                IsActive = offer.IsActive
            };
        }

       
        public async Task<IEnumerable<OfferResultDto>> GetActiveOffersAsync()
        {
            var offerRepo = unitOfWork.GetRepository<Offer, int>();

            var offers = await offerRepo
                .GetQueryable(o =>
                    o.IsActive &&
                    !o.IsDeleted &&
                    o.ExpiresAt > DateTime.UtcNow)
                .Include(o => o.Schedule)
                    .ThenInclude(s => s.Train)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return offers.Select(o => new OfferResultDto
            {
                OfferId = o.Id,
                ScheduleId = o.ScheduleId,
                TrainName = o.Schedule.Train.TrainName!,
                TrainNumber = o.Schedule.Train.TrainNumber!,
                DiscountPercentage = o.DiscountPercentage,
                ExpiresAt = o.ExpiresAt,
                IsActive = o.IsActive
            }).ToList();
        }

        // =====================================================================
        // GET ACTIVE OFFER BY SCHEDULE — بيتستخدم وقت الـ Booking
        // =====================================================================
        public async Task<OfferResultDto?> GetActiveOfferByScheduleAsync(int scheduleId)
        {
            var offerRepo = unitOfWork.GetRepository<Offer, int>();

            var offer = await offerRepo
                .GetQueryable(o =>
                    o.ScheduleId == scheduleId &&
                    o.IsActive &&
                    !o.IsDeleted &&
                    o.ExpiresAt > DateTime.UtcNow)
                .Include(o => o.Schedule)
                    .ThenInclude(s => s.Train)
                .FirstOrDefaultAsync();

            if (offer is null) return null;

            return new OfferResultDto
            {
                OfferId = offer.Id,
                ScheduleId = offer.ScheduleId,
                TrainName = offer.Schedule.Train.TrainName!,
                TrainNumber = offer.Schedule.Train.TrainNumber!,
                DiscountPercentage = offer.DiscountPercentage,
                ExpiresAt = offer.ExpiresAt,
                IsActive = offer.IsActive
            };
        }
    }
}