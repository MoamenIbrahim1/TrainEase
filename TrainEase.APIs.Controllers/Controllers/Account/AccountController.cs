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
        public async Task<ActionResult<RegisterResponseDto>> Register(RegisterDto model)
        {
            var result = await serviceManger.AuthService.RegisterAsync(model);
            return Ok(result);
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            try
            {
                var result = await serviceManger.AuthService.ConfirmEmailAsync(userId, token);

                var html = $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Email Confirmation</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background: #f4f7fb;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
            margin: 0;
        }}
        .card {{
            background: white;
            padding: 40px;
            border-radius: 16px;
            box-shadow: 0 10px 30px rgba(0,0,0,0.1);
            text-align: center;
            max-width: 500px;
            width: 90%;
        }}
        .success {{
            color: #16a34a;
            font-size: 24px;
            font-weight: bold;
            margin-bottom: 15px;
        }}
        .message {{
            color: #333;
            font-size: 18px;
        }}
    </style>
</head>
<body>
    <div class=""card"">
        <div class=""success"">✅ Email Confirmed</div>
        <div class=""message"">{result}</div>
    </div>
</body>
</html>";

                return Content(html, "text/html");
            }
            catch (Exception ex)
            {
                var html = $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Email Confirmation Failed</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background: #fef2f2;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
            margin: 0;
        }}
        .card {{
            background: white;
            padding: 40px;
            border-radius: 16px;
            box-shadow: 0 10px 30px rgba(0,0,0,0.1);
            text-align: center;
            max-width: 500px;
            width: 90%;
        }}
        .error {{
            color: #dc2626;
            font-size: 24px;
            font-weight: bold;
            margin-bottom: 15px;
        }}
        .message {{
            color: #333;
            font-size: 18px;
        }}
    </style>
</head>
<body>
    <div class=""card"">
        <div class=""error"">❌ Confirmation Failed</div>
        <div class=""message"">{ex.Message}</div>
    </div>
</body>
</html>";

                return Content(html, "text/html");
            }
        }

    }
}