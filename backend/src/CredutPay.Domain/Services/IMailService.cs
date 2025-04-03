using CredutPay.Domain.Services.Mail;

namespace CredutPay.Domain.Services
{
    public interface IMailService
    {
        void SendMail(MailMessage mailMessage);
    }
}
