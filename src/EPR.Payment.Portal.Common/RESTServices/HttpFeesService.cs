using EPR.Payment.Portal.Common.Dtos;
using EPR.Payment.Portal.Common.RESTServices.Interfaces;

namespace EPR.Payment.Portal.Common.RESTServices
{
    public class HttpFeesService : IHttpFeesService
    {
        public Task<GetFeesResponseDto> GetFee(bool isLarge, string? regulator)
        {
            throw new NotImplementedException();
        }
    }
}
