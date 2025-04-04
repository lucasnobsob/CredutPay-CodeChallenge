namespace CredutPay.Domain.Models
{
    public class SeedHistory
    {
        public Guid Id { get; set; }
        public string SeedName { get; set; } = string.Empty;
        public DateTime ExecutedAt { get; set; } = DateTime.Now;
    }
}
