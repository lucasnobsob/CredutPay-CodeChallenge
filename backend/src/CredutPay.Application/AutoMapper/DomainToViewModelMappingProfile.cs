using CredutPay.Domain.Models;
using AutoMapper;
using CredutPay.Application.ViewModels;

namespace CredutPay.Application.AutoMapper
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<ApplicationUser, UserViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
                .ReverseMap();

            CreateMap<Wallet, WalletViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => Guid.Parse(src.UserId)))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted))
                .ReverseMap();

            CreateMap<Transaction, TransactionViewModel>()
                .ForMember(x => x.TransactionType, y => y.MapFrom(z => z.Type.ToString()))
                .ReverseMap();

            CreateMap<Transaction, CreateTransactionViewModel>().ReverseMap();
        }
    }
}
