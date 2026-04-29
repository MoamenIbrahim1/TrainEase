using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainEase.Core.Application.Abstrations.Model.Auth
{
    public class UpdateProfileDto
    {
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
    }

}
