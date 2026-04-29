using TrainEase.Core.Application.Abstrations.Model.Auth;

namespace TrainEase.Core.Application.Abstrations.Services
{
    public interface IAuthService
    {
        Task<UserDto> LoginAsync(LoginDto model);
        Task<RegisterResponseDto> RegisterAsync(RegisterDto model);
        Task<string> ConfirmEmailAsync(string userId, string token);
        Task<ProfileDto> GetProfileAsync(string userId);
        Task<ProfileDto> UpdateProfileAsync(string userId, UpdateProfileDto model);
        Task ForgotPasswordAsync(ForgotPasswordDto model);
        Task ResetPasswordAsync(ResetPasswordDto model);
    }
}