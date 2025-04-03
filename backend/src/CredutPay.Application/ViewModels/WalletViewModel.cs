using System.Text.Json.Serialization;

namespace CredutPay.Application.ViewModels
{
    public class WalletViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Balance { get; set; } = 0m;

        [JsonIgnore]
        public bool IsDeleted { get; set; }

        [JsonIgnore]
        public Guid UserId { get; set; }

        [JsonIgnore]
        public UserViewModel User { get; set; } = new UserViewModel();
    }
}
