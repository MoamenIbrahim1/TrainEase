using Microsoft.AspNetCore.Identity;
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
        public async Task<StationMasterDto> AssignStationMasterAsync(CreateStationMasterDto stationMasterDto)
        {
            if (await _userManager.FindByEmailAsync(stationMasterDto.Email) is not null)
                throw new NotFoundException("User with this email already exists.", stationMasterDto.Email);

            var stationMaster = new User
            {
                FullName = stationMasterDto.FullName,
                Email = stationMasterDto.Email,
                UserName = stationMasterDto.Email,
                PhoneNumber = stationMasterDto.PhoneNumber,
                NationalId = stationMasterDto.NationalId,
                Role = UserRole.StationMaster,
                StationId = stationMasterDto.AssignedStationId,
            };
            // Create the user with the specified password
            var result = await _userManager.CreateAsync(stationMaster, stationMasterDto.Password);
            if (!result.Succeeded)
            {
                throw new ValidationException
                {
                    Errors = result.Errors.Select(e => e.Description)
                };
            }
            // Add the user to the StationMaster role
            await _userManager.AddToRoleAsync(stationMaster, UserRole.StationMaster.ToString());

            var role = await _userManager.GetRolesAsync(stationMaster);
            var station = await _unitOfWork.GetRepository<Station, int>().GetAsync(stationMaster.StationId??0);
            return new StationMasterDto
            {
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
            var stationMaster =  await _userManager.FindByIdAsync(stationMasterId);
            if (stationMaster == null || stationMaster.Role != UserRole.StationMaster)
                throw new NotFoundException("Station master not found.", stationMasterId);

            var result = await _userManager.DeleteAsync(stationMaster);
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

