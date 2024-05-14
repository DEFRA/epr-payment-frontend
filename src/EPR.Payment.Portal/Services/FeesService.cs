using AutoMapper;
using EPR.Payment.Portal.Common.Models;
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

        public async Task<GetFeesResponseViewModel> GetFee(bool isLarge, string? regulator)
        {
            var result = await _httpFeeService.GetFee(isLarge, regulator);
            return _mapper.Map<GetFeesResponseViewModel>(result);
        }
    }
}
