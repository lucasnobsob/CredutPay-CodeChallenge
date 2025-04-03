using CredutPay.Domain.Core.Models;

namespace CredutPay.Domain.Models
{
    public class Wallet : Entity
    {
        public Wallet(Guid id, string name, decimal balance, string userId)
        {
            Id = id;
            Name = name;
            Balance = balance;
            UserId = userId;
        }
        public Wallet(string name, string userId)
        {
            Name = name;
            UserId = userId;
        }

        public Wallet() { }

        public string Name { get; set; }
        public decimal Balance { get; set; } = 0m;

        //Foreign Keys
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public ICollection<Transaction> SourceTransactions { get; set; } = new List<Transaction>();
        public ICollection<Transaction> DestinationTransactions { get; set; } = new List<Transaction>();
    }
}
