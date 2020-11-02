using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Core.Services
{
    public abstract class ConferenceService : IConferenceService
    {
        public virtual ValueTask DisposeAsync()
        {
            return new ValueTask();
        }

        public virtual ValueTask InitializeParticipant(Participant participant)
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

        public virtual ValueTask InitializeAsync()
        {
            return new ValueTask();
        }
    }
}