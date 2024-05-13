using AutoMapper;
using EPR.Payment.Portal.Common.Dtos;
using EPR.Payment.Portal.Common.RESTServices.Interfaces;
using EPR.Payment.Portal.Services.Interfaces;

namespace EPR.Payment.Portal.Services
{
    public class FeesService : IFeesService
    {
        private readonly IHttpFeesService _httpFeeService;
        private IMapper _mapper;

        public FeesService(IMapper mapper, IHttpFeesService httpFeeService)
        {
            _mapper = mapper;
            _httpFeeService = httpFeeService ?? throw new ArgumentNullException(nameof(httpFeeService));
        }

        public async Task<GetFeesResponseDto> GetFee(bool isLarge, string? regulator)
        {
            return  await _httpFeeService.GetFee(isLarge, regulator);
        }
    }
}
