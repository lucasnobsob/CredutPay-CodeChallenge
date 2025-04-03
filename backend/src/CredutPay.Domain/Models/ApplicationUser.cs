using Microsoft.AspNetCore.Identity;

namespace CredutPay.Domain.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsDeleted { get; set; }
        public ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
    }
}
