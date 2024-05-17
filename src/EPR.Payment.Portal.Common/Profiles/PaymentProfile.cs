using AutoMapper;
using EPR.Payment.Portal.Common.Dtos.Request;
using EPR.Payment.Portal.Common.Dtos.Response;
using EPR.Payment.Portal.Common.Models.Request;
using EPR.Payment.Portal.Common.Models.Response;

namespace EPR.Payment.Portal.Common.Profiles
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            CreateMap<PaymentStatusResponseViewModel, PaymentStatusResponseDto>()
                .ReverseMap(); 
            CreateMap<PaymentStatusInsertRequestViewModel, PaymentStatusInsertRequestDto>()
                .ReverseMap();
        }
    }
}
