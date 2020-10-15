using System;
using System.Collections.Immutable;
using PaderConference.Core.Dto.UseCaseResponses;
using PaderConference.Core.Interfaces;

namespace PaderConference.Core.Dto.UseCaseRequests
{
    public class CreateConferenceRequest : IUseCaseRequest<CreateConferenceResponse>
    {
        public CreateConferenceRequest(string? name, string conferenceType, IImmutableList<string> organizers,
            DateTimeOffset? startTime, string? scheduleCron, IImmutableDictionary<string, string> permissions)
        {
            Name = name;
            ConferenceType = conferenceType;
            Organizers = organizers;
            StartTime = startTime;
            ScheduleCron = scheduleCron;
            Permissions = permissions;
        }

        public string? Name { get; }

        public string ConferenceType { get; }

        public IImmutableList<string> Organizers { get; }

        public DateTimeOffset? StartTime { get; }

        public string? ScheduleCron { get; }

        public IImmutableDictionary<string, string> Permissions { get; }
    }
}