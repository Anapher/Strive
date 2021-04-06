using System.Collections.Generic;
using System.Threading.Tasks;
using Strive.Core.Services.ConferenceControl;

namespace Strive.Core.Services.ParticipantsList.Gateways
{
    public interface IParticipantMetadataRepository
    {
        ValueTask<IReadOnlyDictionary<string, ParticipantMetadata>> GetParticipantsOfConference(string conferenceId);

        ValueTask AddParticipant(Participant participant, ParticipantMetadata data);

        ValueTask RemoveParticipant(Participant participant);

        ValueTask<ParticipantMetadata?> GetParticipantMetadata(Participant participant);
    }
}
