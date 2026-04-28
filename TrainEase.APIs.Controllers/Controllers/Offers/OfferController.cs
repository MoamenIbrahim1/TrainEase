using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TrainEase.APIs.Controllers.Controllers.Base;
using TrainEase.Core.Application.Abstrations.Model.Offers;
using TrainEase.Core.Application.Abstrations.Services;

namespace TrainEase.APIs.Controllers.Controllers.Offers
{
    public class OfferController(IServiceManager _serviceManager) : BaseApiController
    {
        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateOffer([FromBody] CreateOfferDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _serviceManager.OfferService.CreateOfferAsync(dto, userId);
            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveOffers()
        {
            var result = await _serviceManager.OfferService.GetActiveOffersAsync();
            return Ok(result);
        }
    }
}
