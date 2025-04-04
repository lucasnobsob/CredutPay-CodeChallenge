namespace CredutPay.Application.Interfaces
{
    public interface IPaginatedSuccessResult
    {
        public bool Success { get; set; }
        IEnumerable<object> Data { get; }
        int TotalCount { get; }
    }
}
