using System.Security.Claims;

namespace CredutPay.Infra.CrossCutting.Identity.Services
{
    public interface IJwtFactory
    {
        Task<JwtToken> GenerateJwtToken(ClaimsIdentity claimsIdentity);
    }
}
