using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainEase.Core.Application.Abstrations.Model.StationMaster
{
    public class CreateStationMasterDto
    {
        public required string FullName { get; set; }
        public required string PhoneNumber { get; set; }
        public required string NationalId { get; set; }
        public required string Email { get; set; }
        public int AssignedStationId { get; set; }
        public required string Password { get; set; }
    }
}
