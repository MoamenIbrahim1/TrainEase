using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TrainEase.Core.Application.Abstrations.Model.Auth;
using TrainEase.Core.Application.Abstrations.Services;
using TrainEase.Core.Application.Exceptions;
using TrainEase.Core.Application.Services.Auth;
using TrainEase.Core.Domain.Entities;
using TrainEase.Core.Domain.Entities.Enums;

namespace TrainEase.Core.Application.NewFolder.Auth
{
    public class AuthService(
        IOptions<JwtSettings> jwtSettings,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IEmailService emailService,
        IConfiguration configuration) : IAuthService
    {
        private readonly JwtSettings _jwtSettings = jwtSettings.Value;
        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly IEmailService _emailService = emailService;
        private readonly IConfiguration _configuration = configuration;

        public async Task<UserDto> LoginAsync(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null)
                throw new UnAuthorizedException("Invalid email or password");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (result.IsNotAllowed)
                throw new UnAuthorizedException("Account not confirmed yet.");

            if (!result.Succeeded)
                throw new UnAuthorizedException("Invalid login.");

            return await CreateUserDtoAsync(user);
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterDto model)
        {
            if (await EmailExists(model.Email))
                throw new BadHttpRequestException("This email is already in use.");

            var user = new User
            {
                FullName = model.FullName,
                NationalId = model.NationalId,
                Email = model.Email,
                UserName = model.Email,
                PhoneNumber = model.PhoneNumber,
                Role = UserRole.Passenger
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                throw new ValidationException
                {
                    Errors = result.Errors.Select(e => e.Description)
                };

            await _userManager.AddToRoleAsync(user, UserRole.Passenger.ToString());

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var baseUrl = _configuration["AppSettings:BaseUrl"];

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new BadHttpRequestException("AppSettings:BaseUrl is missing from configuration.");

            var confirmationLink =
                $"{baseUrl}/api/Account/confirm-email?userId={user.Id}&token={encodedToken}";

            var message = $"""
    <div style="font-family: Arial, sans-serif; background-color:#f4f7fb; padding:40px 20px; direction:ltr;">
        <div style="max-width:600px; margin:0 auto; background:#ffffff; border-radius:12px; padding:32px; box-shadow:0 4px 14px rgba(0,0,0,0.08);">
            <h2 style="margin-top:0; color:#1e3a8a;">Welcome to TrainEase 🚆</h2>

            <p style="font-size:16px; color:#111827;">Hello {user.FullName},</p>

            <p style="font-size:15px; color:#374151; line-height:1.8;">
                Your account has been created successfully.
            </p>

            <p style="font-size:15px; color:#374151; line-height:1.8;">
                Please confirm your email by clicking the button below:
            </p>

            <div style="margin:30px 0; text-align:center;">
                <a href="{confirmationLink}"
                   style="display:inline-block; padding:14px 28px; background-color:#2563eb; color:#ffffff; text-decoration:none; border-radius:8px; font-size:16px; font-weight:bold;">
                    Confirm Email
                </a>
            </div>

            <p style="font-size:14px; color:#6b7280; line-height:1.7;">
                If you did not create this account, you can safely ignore this email.
            </p>

            <hr style="border:none; border-top:1px solid #e5e7eb; margin:24px 0;" />

            <p style="font-size:14px; color:#6b7280; margin-bottom:0;">
                Best regards,<br />
                <strong>TrainEase Team</strong>
            </p>
        </div>
    </div>
    """;

            await _emailService.SendEmailAsync(
                user.Email!,
                "Confirm your TrainEase account",
                message);

            return new RegisterResponseDto
            {
                Message = "Account created successfully. Please check your email to confirm your account."
            };
        }

        public async Task<string> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                throw new BadHttpRequestException("Invalid user.");

            if (user.EmailConfirmed)
                return "Email is already confirmed.";

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                throw new BadHttpRequestException(errors);
            }

            return "Email confirmed successfully.";
        }

        public async Task<bool> EmailExists(string email)
        {
            return await _userManager.FindByEmailAsync(email) is not null;
        }

        private async Task<UserDto> CreateUserDtoAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            return new UserDto
            {
                Id = user.Id,
                FullName = user.FullName!,
                NationalId = user.NationalId!,
                Email = user.Email!,
                Token = await GenerateTokenAsync(user, roles),
                Role = roles.FirstOrDefault() ?? string.Empty
            };
        }

        private async Task<string> GenerateTokenAsync(User user, IList<string> roles)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);

            var rolesAsClaims = roles
                .Select(role => new Claim(ClaimTypes.Role, role))
                .ToList();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.PrimarySid, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.FullName!),
                new Claim(ClaimTypes.GivenName, user.FullName!)
            }
            .Union(userClaims)
            .Union(rolesAsClaims);

            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                claims: claims,
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}