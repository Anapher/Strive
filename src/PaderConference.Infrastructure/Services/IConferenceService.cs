using System;
using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Infrastructure.Services
{
    public interface IConferenceService : IAsyncDisposable
    {
        ValueTask OnClientDisconnected(Participant participant);

        ValueTask OnClientConnected(Participant participant);
    }
}