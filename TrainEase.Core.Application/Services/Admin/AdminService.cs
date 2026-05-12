using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Application.Abstrations.Model.StationMaster;
using TrainEase.Core.Application.Abstrations.Services;
using TrainEase.Core.Application.Exceptions;
using TrainEase.Core.Domain.Contracts;
using TrainEase.Core.Domain.Entities;
using TrainEase.Core.Domain.Entities.Enums;

namespace TrainEase.Core.Application.Services.Admin
{
    public class AdminService(UserManager<User> _userManager, IUnitOfWork _unitOfWork) : IAdminService
    {
        public async Task<IEnumerable<StationMasterDto>> GetAllStationMastersAsync()
        {
            // ✅ استثني المحذوفين
            var stationMasters = _userManager.Users
                .Where(u => u.Role == UserRole.StationMaster && !u.IsDeleted)
                .ToList();

            var stationMasterDtos = new List<StationMasterDto>();
            foreach (var stationMaster in stationMasters)
            {
                var role = (await _userManager.GetRolesAsync(stationMaster)).FirstOrDefault();
                var station = await _unitOfWork.GetRepository<Station, int>()
                    .GetAsync(stationMaster.StationId ?? 0);

                stationMasterDtos.Add(new StationMasterDto
                {
                    Id = stationMaster.Id,
                    Name = stationMaster.FullName,
                    Email = stationMaster.Email,
                    NationalId = stationMaster.NationalId,
                    PhoneNumber = stationMaster.PhoneNumber,
                    Role = role ?? string.Empty,
                    StationName = station?.Name ?? string.Empty
                });
            }
            return stationMasterDtos;
        }

        public async Task<StationMasterDto> AssignStationMasterAsync(CreateStationMasterDto stationMasterDto)
        {
            // تحقق إن الإيميل مش موجود
            var existingUser = await _userManager.FindByEmailAsync(stationMasterDto.Email);
            if (existingUser is not null && !existingUser.IsDeleted)
                throw new NotFoundException("User with this email already exists.", stationMasterDto.Email);

            //  تحقق إن المحطة معندهاش ناظر خالص
            var stationAlreadyHasMaster = _userManager.Users.Any(u =>
                u.Role == UserRole.StationMaster &&
                !u.IsDeleted &&
                u.StationId == stationMasterDto.AssignedStationId);

            if (stationAlreadyHasMaster)
                throw new ValidationException
                {
                    Errors = ["This station already has a station master assigned."]
                };

            var stationMaster = new User
            {
                FullName = stationMasterDto.FullName,
                Email = stationMasterDto.Email,
                UserName = stationMasterDto.Email,
                PhoneNumber = stationMasterDto.PhoneNumber,
                NationalId = stationMasterDto.NationalId,
                Role = UserRole.StationMaster,
                StationId = stationMasterDto.AssignedStationId,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(stationMaster, stationMasterDto.Password);
            if (!result.Succeeded)
            {
                throw new ValidationException
                {
                    Errors = result.Errors.Select(e => e.Description)
                };
            }

            await _userManager.AddToRoleAsync(stationMaster, UserRole.StationMaster.ToString());

            var role = await _userManager.GetRolesAsync(stationMaster);
            var station = await _unitOfWork.GetRepository<Station, int>()
                .GetAsync(stationMaster.StationId ?? 0);

            return new StationMasterDto
            {
                Id = stationMaster.Id,
                Name = stationMaster.FullName,
                Email = stationMaster.Email,
                NationalId = stationMaster.NationalId,
                PhoneNumber = stationMaster.PhoneNumber,
                Role = role.FirstOrDefault()?.ToString() ?? string.Empty,
                StationName = station?.Name ?? string.Empty
            };
        }

        public async Task<bool> RemoveStationMasterAsync(string stationMasterId)
        {
            var stationMaster = await _userManager.FindByIdAsync(stationMasterId);

            if (stationMaster == null || stationMaster.IsDeleted || stationMaster.Role != UserRole.StationMaster)
                throw new NotFoundException("Station master not found.", stationMasterId);

            // ✅ Soft Delete + Unassign من المحطة
            stationMaster.IsDeleted = true;
            stationMaster.StationId = null;

            var result = await _userManager.UpdateAsync(stationMaster);
            if (!result.Succeeded)
            {
                throw new ValidationException
                {
                    Errors = result.Errors.Select(e => e.Description)
                };
            }

            return true;
        }
    }
}

