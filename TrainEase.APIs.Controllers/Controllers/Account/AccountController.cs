using Microsoft.AspNetCore.Mvc;
using TrainEase.APIs.Controllers.Controllers.Base;
using TrainEase.Core.Application.Abstrations.Model.Auth;
using TrainEase.Core.Application.Abstrations.Services;

namespace TrainEase.APIs.Controllers.Controllers.Account
{

    public class AccountController(IServiceManager serviceManger) : BaseApiController
    {
        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var result = await serviceManger.AuthService.LoginAsync(model);
            return Ok(result);

        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto model)
        {
            var result = await serviceManger.AuthService.RegisterAsync(model);
            return Ok(result);
        }
    }
}
