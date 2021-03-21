using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Core.Services.Media.Dtos;

namespace PaderConference.Core.Services.Media.Gateways
{
    public interface IMediaStateRepository
    {
        ValueTask Set(string conferenceId, IReadOnlyDictionary<string, ParticipantStreams> value);

        ValueTask<IReadOnlyDictionary<string, ParticipantStreams>> Get(string conferenceId);

        ValueTask Remove(string conferenceId);
    }
}
