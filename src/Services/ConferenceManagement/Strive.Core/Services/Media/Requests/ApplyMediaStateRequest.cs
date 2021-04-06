using System.Collections.Generic;
using MediatR;
using Strive.Core.Services.Media.Dtos;

namespace Strive.Core.Services.Media.Requests
{
    public record ApplyMediaStateRequest
        (string ConferenceId, IReadOnlyDictionary<string, ParticipantStreams> Payload) : IRequest<Unit>;
}
