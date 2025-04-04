namespace CredutPay.Application.Interfaces
{
    public interface IPaginatedResult
    {
        IEnumerable<object> Items { get; }
        int TotalCount { get; }
    }
}
