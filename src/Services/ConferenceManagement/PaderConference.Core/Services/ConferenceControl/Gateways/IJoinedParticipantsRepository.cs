using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaderConference.Core.Services.ConferenceControl.Gateways
{
    public interface IJoinedParticipantsRepository
    {
        /// <summary>
        ///     Register a participant in the mapping
        /// </summary>
        /// <returns>Return the conference id that was previously set or null if the participant did not belong to a conference</returns>
        Task<string?> RegisterParticipant(string participantId, string conferenceId, JoinedParticipantData data);

        /// <summary>
        ///     Remove the participant from the mapping
        /// </summary>
        /// <returns>Return the conference id that was previously set or null if the participant did not belong to a conference</returns>
        Task<string?> RemoveParticipant(string participantId);

        Task<string?> GetConferenceIdOfParticipant(string participantId);

        Task<IReadOnlyDictionary<string, JoinedParticipantData>> GetParticipantsOfConference(string conferenceId);
    }
}
