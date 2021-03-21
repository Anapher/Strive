using System.Collections.Generic;
using MediatR;
using PaderConference.Core.Services.Media.Dtos;

namespace PaderConference.Core.Services.Media.Requests
{
    public record ApplyMediaStateRequest
        (string ConferenceId, IReadOnlyDictionary<string, ParticipantStreams> Payload) : IRequest<Unit>;
}
