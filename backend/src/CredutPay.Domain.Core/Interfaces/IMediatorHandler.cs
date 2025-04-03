using CredutPay.Domain.Core.Commands;
using CredutPay.Domain.Core.Events;

namespace CredutPay.Domain.Core.Interfaces
{
    public interface IMediatorHandler
    {
        Task SendCommand<T>(T command) where T : Command;
        Task RaiseEvent<T>(T @event) where T : Event;
    }
}
