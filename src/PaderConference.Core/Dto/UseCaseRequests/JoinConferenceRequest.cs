using System;
using System.Threading;
using System.Threading.Tasks;
using PaderConference.Core.Dto.UseCaseResponses;
using PaderConference.Core.Interfaces;

namespace PaderConference.Core.Dto.UseCaseRequests
{
    public class JoinConferenceRequest : IUseCaseRequest<JoinConferenceResponse>
    {
        public JoinConferenceRequest(string conferenceId, string participantId, string role, string? name,
            string connectionId, CancellationToken cancellationToken, Func<Task> enableParticipantMessaging)
        {
            ConferenceId = conferenceId;
            ParticipantId = participantId;
            Role = role;
            Name = name;
            ConnectionId = connectionId;
            CancellationToken = cancellationToken;
            EnableParticipantMessaging = enableParticipantMessaging;
        }

        public string ConferenceId { get; }

        public string ParticipantId { get; }

        public string Role { get; }

        public string? Name { get; }

        public string ConnectionId { get; }

        public CancellationToken CancellationToken { get; }

        public Func<Task> EnableParticipantMessaging { get; }
    }
}
