using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TrainEase.Core.Application.Abstrations.Model.Train;
using TrainEase.Core.Application.Abstrations.Services;
using TrainEase.Core.Application.Exceptions;
using TrainEase.Core.Domain.Contracts;
using TrainEase.Core.Domain.Entities;

namespace TrainEase.Core.Application.Services.Trains
{
    public class TrainService(IUnitOfWork _unitOfWork, IMapper _mapper) : ITrainService
    {
        public async Task<IEnumerable<TrainDto>> GetTrainsAsync()
        {
            var trains = await _unitOfWork.GetRepository<Train, int>().GetAllAsync();
            var result = _mapper.Map<IEnumerable<TrainDto>>(trains);
            return result;
        }
        public async Task<bool> AddTrainAsync(AddTrainDto addTrainDto)
        {
            if (addTrainDto == null)
                throw new ArgumentNullException(nameof(addTrainDto));

            if (_unitOfWork.GetRepository<Train, int>().GetQueryable(t => t.TrainNumber == addTrainDto.TrainNumber).Any())
                throw new BadRequestException($"a train with the number {addTrainDto.TrainNumber} already exists");
            var train = new Train
            {

                TrainName = addTrainDto.Name,
                Type = addTrainDto.TrainType,
                TrainNumber = addTrainDto.TrainNumber,
                CreatedAt = DateTime.UtcNow
            };
            for (int i = 1; i <= addTrainDto.Carriages; i++)
            {
                string classType;

                if (i <= 2)
                    classType = "First Class";
                else if (i == addTrainDto.Carriages)
                    classType = "VIP";
                else
                    classType = "Second Class";

                train.Carriages.Add(new Carriage
                {
                    CarriageNumber = $"C{i}",
                    ClassType = classType
                });
            }
            await _unitOfWork.GetRepository<Train, int>().AddAsync(train);
            var created = await _unitOfWork.CompeleteAsync() > 0;

            if (!created) throw new BadRequestException("an error has been occuered when creating the train");
            return true;
        }

        public async Task<bool> DeleteTrainAsync(int trainId)
        {
            var train = await _unitOfWork.GetRepository<Train, int>().GetAsync(trainId);

            if (train == null)
                throw new NotFoundException($"{train!.TrainName}", trainId);

            _unitOfWork.GetRepository<Train, int>().Delete(train);

            var deleted = await _unitOfWork.CompeleteAsync() > 0;
            if (!deleted) throw new BadRequestException("an error has been occuered when deleting the train");

            return true;
        }

        public async Task<TrainDto> EditTrainAsync(int id, EditTrainDto editTrainDto)
        {
            if (editTrainDto == null)
                throw new ArgumentNullException(nameof(editTrainDto));

            var trainRepo = _unitOfWork.GetRepository<Train, int>();

            var train = await trainRepo
                .GetQueryable(t => t.Id == id)
                .Include(t => t.Carriages)
                .FirstOrDefaultAsync();

            if (train == null)
                throw new NotFoundException("Train not found", id);

            //  Check TrainNumber uniqueness 
            if (!string.IsNullOrEmpty(editTrainDto.TrainNumber))
            {
                if (trainRepo
                    .GetQueryable(t => t.TrainNumber == editTrainDto.TrainNumber && t.Id != id)
                    .Any())
                {
                    throw new BadRequestException($"Train number {editTrainDto.TrainNumber} already exists");
                }

                train.TrainNumber = editTrainDto.TrainNumber;
            }

            //  Update only if values exist
            if (!string.IsNullOrEmpty(editTrainDto.TrainName))
                train.TrainName = editTrainDto.TrainName;

            if (!string.IsNullOrEmpty(editTrainDto.TrainType))
                train.Type = editTrainDto.TrainType;

            train.UpdatedAt = DateTime.UtcNow;

            //  Handle Carriages 
            if (editTrainDto.Carriages.HasValue)
            {
                var currentCount = train.Carriages.Count;
                var newCount = editTrainDto.Carriages.Value;

                if (newCount > currentCount)
                {
                    for (int i = currentCount + 1; i <= newCount; i++)
                    {
                        string classType = i switch
                        {
                            <= 2 => "First Class",
                            var x when x == newCount => "VIP",
                            _ => "Second Class"
                        };

                        train.Carriages.Add(new Carriage
                        {
                            CarriageNumber = $"C{i}",
                            ClassType = classType
                        });
                    }
                }
                else if (newCount < currentCount)
                {
                    var toRemove = train.Carriages
                        .OrderByDescending(c => c.Id) 
                        .Take(currentCount - newCount)
                        .ToList();

                    foreach (var carriage in toRemove)
                    {
                        train.Carriages.Remove(carriage);
                    }
                }
            }

            trainRepo.Update(train);

            var updated = await _unitOfWork.CompeleteAsync() > 0;

            if (!updated)
                throw new BadRequestException("Error while updating train");

            return new TrainDto
            {
                TrainName = train.TrainName!,
                TrainNumber = train.TrainNumber!,
                TrainType = train.Type!,
                Carriages = train.Carriages.Count
            };
        }
    }
}