using EPR.Payment.Portal.Common.Dtos;
using EPR.Payment.Portal.Common.Models;
using EPR.Payment.Portal.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EPR.Payment.Portal.Controllers
{
    [Route("[controller]")]
    public class FeesController : Controller
    {
        private readonly IFeesService _feesService;
        public FeesController(IFeesService feesService) 
        {
            _feesService = feesService;
        }

        [HttpGet("Fee")]
        public async Task<IActionResult> GetFee(bool isLarge, string? regulator)
        {
            // Check for invalid parameters
            if (string.IsNullOrEmpty(regulator))
            {
                return BadRequest("Invalid 'regulator' parameter provided");
            }

            var feeResponseVm = await _feesService.GetFee(isLarge, regulator);

            return View(feeResponseVm);
        }
    }
}
