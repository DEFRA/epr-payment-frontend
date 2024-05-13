using EPR.Payment.Portal.Common.Dtos;

namespace EPR.Payment.Portal.Common.RESTServices.Interfaces
{
    public interface IHttpFeesService
    {
        Task<GetFeesResponseDto> GetFee(bool isLarge, string? regulator);
    }
}
