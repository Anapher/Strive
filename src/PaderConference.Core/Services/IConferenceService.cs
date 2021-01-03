using System;
using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Core.Services
{
    public interface IConferenceService : IAsyncDisposable
    {
        /// <summary>
        ///     Initialize this service. This method is called once after creation.
        /// </summary>
        ValueTask InitializeAsync();

        /// <summary>
        ///     A new client connected. This method may be used to register that participant internally. The participant must not
        ///     receive data here.
        /// </summary>
        /// <param name="participant">The participant that connected</param>
        ValueTask OnClientConnected(Participant participant);

        /// <summary>
        ///     Initialize a newly connected participant, e.g. send initial data
        /// </summary>
        /// <param name="participant">The participant</param>
        ValueTask InitializeParticipant(Participant participant);

        /// <summary>
        ///     A client disconnected. Please note that it is not necessary that <see cref="OnClientConnected" /> or
        ///     <see cref="InitializeParticipant" /> was called before.
        /// </summary>
        /// <param name="participant">The participant that disconnected</param>
        ValueTask OnClientDisconnected(Participant participant);
    }
}