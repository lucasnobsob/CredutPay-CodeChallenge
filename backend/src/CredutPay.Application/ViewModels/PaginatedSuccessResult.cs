using CredutPay.Application.Interfaces;

namespace CredutPay.Application.ViewModels
{
    public class PaginatedSuccessResult<T> : IPaginatedSuccessResult
    {
        public bool Success { get; set; }
        public IEnumerable<T> Data { get; set; }
        public int TotalCount { get; set; }

        IEnumerable<object> IPaginatedSuccessResult.Data => Data.Cast<object>();

        public PaginatedSuccessResult(bool success, IEnumerable<T> data, int totalCount)
        {
            Success = success;
            Data = data;
            TotalCount = totalCount;
        }

        public PaginatedSuccessResult(IEnumerable<T> data, int totalCount)
        {
            Data = data;
            TotalCount = totalCount;
        }
    }
}
