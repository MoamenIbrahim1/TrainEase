using TrainEase.Core.Application.Abstrations.Model.Carriage;

namespace TrainEase.Core.Application.Abstrations.Model.Train
{
    public class AddTrainDto
    {
        public required string TrainNumber { get; set; }
        public required string Name { get; set; }
        public required string TrainType { get; set; }
        public required List<AddCarriageDto> Carriages { get; set; }
    }
}
