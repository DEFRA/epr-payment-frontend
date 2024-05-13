using EPR.Payment.Portal.Common.Dtos;

namespace EPR.Payment.Portal.Services.Interfaces
{
    public interface IFeesService
    {
        Task<GetFeesResponseDto> GetFee(bool isLarge, string? regulator);
    }
}
