using Refit;

namespace CredutPay.Domain.Services
{
    public interface IFooClient
    {
        [Get("/")]
        Task<object> GetAll();
    }
}
