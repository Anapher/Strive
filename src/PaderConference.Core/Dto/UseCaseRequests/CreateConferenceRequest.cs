using System;
using System.Collections.Immutable;
using PaderConference.Core.Dto.UseCaseResponses;
using PaderConference.Core.Interfaces;

namespace PaderConference.Core.Dto.UseCaseRequests
{
    public class CreateConferenceRequest : IUseCaseRequest<CreateConferenceResponse>
    {
        public CreateConferenceRequest(string? name, string conferenceType, IImmutableList<string> organizers,
            DateTimeOffset? startTime, DateTimeOffset? endTime, string? scheduleCron,
            IImmutableDictionary<string, string> permissions)
        {
            Name = name;
            ConferenceType = conferenceType;
            Organizers = organizers;
            StartTime = startTime;
            EndTime = endTime;
            ScheduleCron = scheduleCron;
            Permissions = permissions;
        }

        public string? Name { get; }

        public string ConferenceType { get; }

        public IImmutableList<string> Organizers { get; }

        public DateTimeOffset? StartTime { get; }

        public DateTimeOffset? EndTime { get; set; }

        public string? ScheduleCron { get; }

        public IImmutableDictionary<string, string> Permissions { get; }
    }
}