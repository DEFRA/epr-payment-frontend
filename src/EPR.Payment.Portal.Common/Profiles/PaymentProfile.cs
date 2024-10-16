using AutoMapper;
using EPR.Payment.Portal.Common.Dtos.Response;
using EPR.Payment.Portal.Common.Models;

namespace EPR.Payment.Portal.Common.Profiles
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            //For now, amount is added as int and decimal in different projects. It will be deleted when decided.
            CreateMap<CompletePaymentResponseDto, CompletePaymentViewModel > ().ForMember(dest => dest.Amount, opt => opt.MapFrom(src => (src.Amount != null ? (int)src.Amount : 0))); 
        }
    }
}
