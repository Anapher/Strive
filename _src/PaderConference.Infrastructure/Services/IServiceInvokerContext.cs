using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Services;

namespace PaderConference.Infrastructure.Services
{
    public interface IServiceInvokerContext
    {
        string ConnectionId { get; }

        IServiceMessage CreateMessage(Participant participant);

        IServiceMessage<T> CreateMessage<T>(T payload, Participant participant);
    }
}
