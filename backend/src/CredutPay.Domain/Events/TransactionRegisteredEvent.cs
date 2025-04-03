using CredutPay.Domain.Core.Events;
using CredutPay.Domain.Models;

namespace CredutPay.Domain.Events
{
    public class TransactionRegisteredEvent : Event
    {
        public TransactionRegisteredEvent(decimal amount, string description, DateTime date)
        {
            Amount = amount;
            Description = description;
            Date = date;
        }

        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }
}
