using System;
using System.Collections.Generic;
using MediatR;

namespace PaderConference.Core.Services.Synchronization.Requests
{
    public record UpdateSynchronizedObjectRequest(string ConferenceId, IEnumerable<string> ParticipantIds,
        Type ProviderType) : IRequest<Unit>
    {
        public static UpdateSynchronizedObjectRequest Create<T>(string conferenceId, IEnumerable<string> participantIds)
            where T : ISynchronizedObjectProvider
        {
            return new(conferenceId, participantIds, typeof(T));
        }
    }
}
