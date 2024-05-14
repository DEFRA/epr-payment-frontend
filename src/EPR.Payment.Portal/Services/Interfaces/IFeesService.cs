using EPR.Payment.Portal.Common.Dtos;
using EPR.Payment.Portal.Common.Models;

namespace EPR.Payment.Portal.Services.Interfaces
{
    public interface IFeesService
    {
        Task<GetFeesResponseViewModel> GetFee(bool isLarge, string? regulator);
    }
}
