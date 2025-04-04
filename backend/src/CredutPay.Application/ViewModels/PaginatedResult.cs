using CredutPay.Application.Interfaces;

namespace CredutPay.Application.ViewModels
{
    public class PaginatedResult<T> : IPaginatedResult
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalCount { get; set; }

        IEnumerable<object> IPaginatedResult.Items => Items.Cast<object>();

        public PaginatedResult(IEnumerable<T> items, int totalCount)
        {
            Items = items;
            TotalCount = totalCount;
        }
    }
}
