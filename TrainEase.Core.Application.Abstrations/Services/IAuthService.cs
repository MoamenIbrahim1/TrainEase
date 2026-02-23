using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Application.Abstrations.Model.Auth;

namespace TrainEase.Core.Application.Abstrations.Services
{
    public interface IAuthService
    {
      Task<UserDto> LoginAsync(LoginDto model);
      Task<UserDto> RegisterAsync(RegisterDto model);

    }
}
