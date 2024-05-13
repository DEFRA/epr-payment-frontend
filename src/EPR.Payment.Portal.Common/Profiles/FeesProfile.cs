using AutoMapper;
using EPR.Payment.Portal.Common.Dtos;
using EPR.Payment.Portal.Common.Models;
using System.Diagnostics.CodeAnalysis;

namespace EPR.Payment.Portal.Common.Profiles
{
    [ExcludeFromCodeCoverage]
    public class FeesProfile : Profile
    {
        public FeesProfile()
        {
            CreateMap<GetFeesResponseViewModel, GetFeesResponseDto>()
                .ReverseMap();
        }
    }
}
