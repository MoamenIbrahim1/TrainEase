using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainEase.Core.Application.Abstrations.Model.Auth
{

    public class UserDto
    {
        public required string Id { get; set; }
        public required string FullName { get; set; }
        public required string NationalId { get; set; }
        public required string Email { get; set; }
        public required string Token { get; set; }
    }
}
