namespace CredutPay.Application.ViewModels
{
    public class CreateTransactionViewModel
    {
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public Guid SourceWalletId { get; set; }
        public Guid DestinationWalletId { get; set; }
    }
}
