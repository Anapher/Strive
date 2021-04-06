using System.Collections.Generic;
using Strive.Core.Services.Media.Dtos;

namespace Strive.Messaging.SFU.ReceiveContracts
{
    public interface StreamsUpdated
    {
        string ConferenceId { get; }

        IReadOnlyDictionary<string, ParticipantStreams> Streams { get; }
    }
}
