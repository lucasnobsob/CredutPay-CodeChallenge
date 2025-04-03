namespace CredutPay.Application.EventSourcedNormalizers
{
    public class WalletHistoryData
    {
        public string Action { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public string When { get; set; }
        public string Who { get; set; }
    }
}
