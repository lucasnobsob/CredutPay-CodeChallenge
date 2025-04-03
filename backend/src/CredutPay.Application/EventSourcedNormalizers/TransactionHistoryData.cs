namespace CredutPay.Application.EventSourcedNormalizers
{
    public class TransactionHistoryData
    {
        public string Action { get; set; }
        public string Id { get; set; }
        public string UserName { get; set; }
        public decimal Amount { get; set; }
        public string When { get; set; }
        public string Who { get; set; }
    }
}
