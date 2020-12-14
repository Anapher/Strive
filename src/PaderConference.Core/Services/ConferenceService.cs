using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Core.Services
{
    public abstract class ConferenceService : IConferenceService
    {
        public virtual ValueTask DisposeAsync()
        {
            return new();
        }

        public virtual ValueTask InitializeParticipant(Participant participant)
        {
            return new();
        }

        public virtual ValueTask OnClientDisconnected(Participant participant)
        {
            return new();
        }

        public virtual ValueTask OnClientConnected(Participant participant)
        {
            return new();
        }

        public virtual ValueTask InitializeAsync()
        {
            return new();
        }
    }
}