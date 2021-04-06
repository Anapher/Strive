using System.Collections.Generic;
using System.Threading.Tasks;
using Strive.Core.Services.Media.Dtos;

namespace Strive.Core.Services.Media.Gateways
{
    public interface IMediaStateRepository
    {
        ValueTask Set(string conferenceId, IReadOnlyDictionary<string, ParticipantStreams> value);

        ValueTask<IReadOnlyDictionary<string, ParticipantStreams>> Get(string conferenceId);

        ValueTask Remove(string conferenceId);
    }
}
