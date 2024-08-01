using AutoMapper;
using EPR.Payment.Portal.Common.Dtos.Response;
using EPR.Payment.Portal.Common.Models;

namespace EPR.Payment.Portal.Common.Profiles
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            CreateMap<CompletePaymentViewModel, CompletePaymentResponseDto>().ReverseMap();
        }
    }
}
