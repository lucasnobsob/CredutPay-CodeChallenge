namespace CredutPay.Application.ViewModels
{
    public class ErrorResult<T>
    {
        public bool Success { get; set; }
        public IEnumerable<T> Errors { get; set; }

        public ErrorResult(bool success, IEnumerable<T> data)
        {
            Success = success;
            Errors = data;
        }
    }
}
