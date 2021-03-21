using System.Collections.Generic;
using PaderConference.Core.Services.Media.Dtos;

namespace PaderConference.Messaging.SFU.ReceiveContracts
{
    public interface StreamsUpdated
    {
        string ConferenceId { get; }

        IReadOnlyDictionary<string, ParticipantStreams> Streams { get; }
    }
}
