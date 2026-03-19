using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        IOptions<JwtSettings>jwtSettings,
        UserManager<User> _userManager, SignInManager<User> _signInManager) : IAuthService
    {
        private readonly JwtSettings _jwtSettings = jwtSettings.Value;
        public async Task<UserDto> LoginAsync(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                throw new UnAuthorizedException("Invalid email or password");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (result.IsNotAllowed) throw new UnAuthorizedException("Account not confirmed yet.");

            if (!result.Succeeded) throw new UnAuthorizedException("Invalid Login.");

            var response = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName!,
                NationalId = user.NationalId!,
                Email = user.Email!,
                Token = await GenerateTokenAsync(user)
            };
            return response;

        }

        public async Task<UserDto> RegisterAsync(RegisterDto model)
        {
            if (await EmailExists(model.Email))
                throw new BadHttpRequestException("This Email is already in use.");

            var user = new User
            {
                FullName = model.FullName,
                NationalId = model.NationalId,
                Email = model.Email,
                UserName = model.Email,
                PhoneNumber = model.PhoneNumber,
                Role = UserRole.Passenger,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded) throw new ValidationException() { Errors = result.Errors.Select(E => E.Description) };

            await _userManager.AddToRoleAsync(user, UserRole.Passenger.ToString());
            var response = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName!,
                NationalId = user.NationalId!,
                Email = user.Email!,
                Token = await GenerateTokenAsync(user)
            };

            return response;
        }

        public async Task<bool> EmailExists(string email)
        {
            return await _userManager.FindByEmailAsync(email) is not null;
        }

        private async Task<string> GenerateTokenAsync(User user)
        {
            // Payload claims for the token  
            var userClaims = await _userManager.GetClaimsAsync(user);

            var roles = await _userManager.GetRolesAsync(user);
            var rolesAsClaims = new List<Claim>();
            foreach (var role in roles)
                rolesAsClaims.Add(new Claim(ClaimTypes.Role, role.ToString()));


            var Claims = new List<Claim>
        {
            new Claim(ClaimTypes.PrimarySid, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.GivenName, user.FullName!),
        }.Union(userClaims).Union(rolesAsClaims);

            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signInCredentials = new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256);

            var tokenObj = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                claims: Claims,
                signingCredentials: signInCredentials
            );
            return new JwtSecurityTokenHandler().WriteToken(tokenObj);
        }
    }
}