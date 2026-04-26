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
            var trainRepo = _unitOfWork.GetRepository<Train, int>();

            var trains = await trainRepo
                .GetQueryable(t => !t.IsDeleted)
                .Include(t => t.Carriages.Where(c => !c.IsDeleted))
                .ToListAsync();

            var result = trains.Select(t => new TrainDto
            {
                Id = t.Id,
                TrainNumber = t.TrainNumber!,
                TrainName = t.TrainName!,
                TrainType = t.Type ?? "",
                CarriagesCount = t.Carriages.Count,
                Classes = t.Carriages
                    .GroupBy(c => c.ClassType)
                    .Select(g => new TrainClassDto
                    {
                        ClassType = g.Key!,
                        CarriagesCount = g.Count()
                    })
                    .ToList(),
                ClassesCount = t.Carriages
                    .Select(c => c.ClassType)
                    .Distinct()
                    .Count()
            }).ToList();

            return result;
        }
        public async Task<bool> AddTrainAsync(AddTrainDto addTrainDto)
        {
            if (addTrainDto is null)
                throw new ArgumentNullException(nameof(addTrainDto));

            if (addTrainDto.Carriages is null || !addTrainDto.Carriages.Any())
                throw new BadRequestException("Train must contain at least one carriage.");

            var trainRepo = _unitOfWork.GetRepository<Train, int>();

            var isExists = await trainRepo
                .GetQueryable(t => t.TrainNumber == addTrainDto.TrainNumber && !t.IsDeleted)
                .AnyAsync();

            if (isExists)
                throw new BadRequestException($"A train with the number {addTrainDto.TrainNumber} already exists.");

            var train = new Train
            {
                TrainName = addTrainDto.Name,
                Type = addTrainDto.TrainType,
                TrainNumber = addTrainDto.TrainNumber,
                CreatedAt = DateTime.UtcNow,
                Carriages = new List<Carriage>()
            };

            foreach (var carriageDto in addTrainDto.Carriages)
            {
                var carriage = new Carriage
                {
                    CarriageNumber = carriageDto.CarriageNumber,
                    ClassType = carriageDto.ClassType,
                    CreatedAt = DateTime.UtcNow,
                    Seats = new List<Seat>()
                };

                for (int i = 1; i <= carriageDto.SeatCount; i++)
                {
                    carriage.Seats.Add(new Seat
                    {
                        SeatNumber = i.ToString(),
                        CreatedAt = DateTime.UtcNow
                    });
                }

                train.Carriages.Add(carriage);
            }

            await trainRepo.AddAsync(train);
            var created = await _unitOfWork.CompeleteAsync() > 0;

            if (!created)
                throw new BadRequestException("An error occurred while creating the train.");

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
                CarriagesCount = train.Carriages.Count
            };
        }
        public async Task<IEnumerable<TrainClassDto>> GetTrainClassesAsync(int trainId)
        {
            var trainRepo = _unitOfWork.GetRepository<Train, int>();

            var train = await trainRepo
                .GetQueryable(t => t.Id == trainId && !t.IsDeleted)
                .Include(t => t.Carriages.Where(c => !c.IsDeleted))
                .FirstOrDefaultAsync();

            if (train is null)
                throw new NotFoundException("Train", trainId);

            var result = train.Carriages
                .GroupBy(c => c.ClassType)
                .Select(g => new TrainClassDto
                {
                    ClassType = g.Key!,
                    CarriagesCount = g.Count()
                })
                .ToList();

            return result;
        }
    }
}