using CredutPay.Domain.Models;
using AutoMapper;
using CredutPay.Application.ViewModels;

namespace CredutPay.Application.AutoMapper
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<Wallet, WalletViewModel>().ReverseMap();

            CreateMap<Transaction, TransactionViewModel>()
                .ForMember(x => x.TransactionType, y => y.MapFrom(z => z.Type.ToString()))
                .ReverseMap();

            CreateMap<Transaction, CreateTransactionViewModel>().ReverseMap();
        }
    }
}
