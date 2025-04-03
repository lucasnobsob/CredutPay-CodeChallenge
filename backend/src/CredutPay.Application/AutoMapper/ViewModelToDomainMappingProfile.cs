using AutoMapper;
using CredutPay.Application.ViewModels;
using CredutPay.Domain.Commands;

namespace CredutPay.Application.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<CreateWalletViewModel, RegisterNewWalletCommand>()
                .ConstructUsing(c => new RegisterNewWalletCommand(c.Name, c.UserId));

            CreateMap<CreateTransactionViewModel, RegisterNewTransactionCommand>()
                .ConstructUsing(c => new RegisterNewTransactionCommand(c.Amount, c.Description, c.SourceWalletId, c.DestinationWalletId));
        }
    }
}
