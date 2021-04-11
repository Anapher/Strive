using System.Collections.Generic;
using System.Threading.Tasks;
using Strive.Infrastructure.KeyValue.Abstractions;

namespace Strive.Core.Services.ConferenceControl.Gateways
{
    public interface IJoinedParticipantsRepository
    {
        /// <summary>
        ///     Register a participant in the mapping
        /// </summary>
        /// <returns>Return the conference id that was previously set or null if the participant did not belong to a conference</returns>
        ValueTask<PreviousParticipantState?> AddParticipant(Participant participant, string connectionId);


        /// <summary>
        ///     Try to remove participant from mapping if the mapping still refers to the connection id. This is important as if a
        ///     client joins and overwrites the mapping of an already joined client, the joined client will be kicked and will try
        ///     to remove the mapping on leave
        /// </summary>
        /// <returns>Return false if the participant was not joined or if he belongs to a different conference</returns>
        ValueTask<bool> RemoveParticipant(string participantId, string connectionId);

        ValueTask<string?> GetConferenceIdOfParticipant(string participantId);

        ValueTask<IEnumerable<Participant>> GetParticipantsOfConference(string conferenceId);

        ValueTask<bool> IsParticipantJoined(Participant participant);

        ValueTask<bool> IsParticipantJoined(Participant participant, string connectionId);

        ValueTask<IAcquiredLock> LockParticipantJoin(Participant participant);
    }
}
