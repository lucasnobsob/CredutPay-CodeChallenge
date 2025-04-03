using System.Text.Json.Serialization;

namespace CredutPay.Application.ViewModels
{
    public class CreateWalletViewModel
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        [JsonIgnore]
        public decimal Balance { get; set; } = 0m;

        [JsonIgnore]
        public Guid UserId { get; set; }
    }
}
