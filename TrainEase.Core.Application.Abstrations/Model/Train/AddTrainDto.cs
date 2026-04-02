namespace TrainEase.Core.Application.Abstrations.Model.Train
{
    public class AddTrainDto
    {
        public required string TrainNumber { get; set; }
        public required string Name { get; set; }
        public required string TrainType { get; set; }
        public required int Carriages { get; set; }
    }
}
