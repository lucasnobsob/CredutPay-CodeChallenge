namespace CredutPay.Application.ViewModels
{
    public class SuccessResult<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }

        public SuccessResult(bool success, T data)
        {
            Success = success;
            Data = data;
        }

    }
}
