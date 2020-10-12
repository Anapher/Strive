using System;
using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Infrastructure.Services
{
    public interface IConferenceService : IAsyncDisposable
    {
        ValueTask InitializeAsync();

        ValueTask OnClientDisconnected(Participant participant, string connectionId);

        ValueTask OnClientConnected(Participant participant);
    }
}