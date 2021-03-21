using System.Collections.Generic;
using PaderConference.Core.Services.Media.Dtos;
using PaderConference.Messaging.SFU.Contracts;

namespace PaderConference.Messaging.SFU.ReceiveContracts
{
    public interface StreamsUpdated : SfuMessage<IReadOnlyDictionary<string, ParticipantStreams>>
    {
    }
}
