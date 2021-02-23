using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Core.Services.ConferenceControl;

namespace PaderConference.Core.Services.ParticipantsList.Gateways
{
    public interface IParticipantMetadataRepository
    {
        ValueTask<IReadOnlyDictionary<string, ParticipantMetadata>> GetParticipantsOfConference(string conferenceId);

        ValueTask AddParticipant(Participant participant, ParticipantMetadata data);

        ValueTask RemoveParticipant(Participant participant);
    }
}
