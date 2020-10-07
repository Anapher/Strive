using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Infrastructure.Services
{
    public abstract class ConferenceService : IConferenceService
    {
        public virtual ValueTask DisposeAsync()
        {
            return new ValueTask();
        }

        public virtual ValueTask OnClientDisconnected(Participant participant)
        {
            return new ValueTask();
        }

        public virtual ValueTask OnClientConnected(Participant participant)
        {
            return new ValueTask();
        }
    }
}