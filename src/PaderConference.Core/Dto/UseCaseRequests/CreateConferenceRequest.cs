using System;
using System.Collections.Immutable;
using PaderConference.Core.Dto.UseCaseResponses;
using PaderConference.Core.Interfaces;

namespace PaderConference.Core.Dto.UseCaseRequests
{
    public class CreateConferenceRequest : IUseCaseRequest<CreateConferenceResponse>
    {
        public CreateConferenceRequest(string? name, string conferenceType, IImmutableList<string> moderators,
            DateTimeOffset? startTime, DateTimeOffset? endTime, string? scheduleCron,
            IImmutableDictionary<string, string>? permissions,
            IImmutableDictionary<string, string>? defaultRoomPermissions,
            IImmutableDictionary<string, string>? moderatorPermissions)
        {
            Name = name;
            ConferenceType = conferenceType;
            Moderators = moderators;
            StartTime = startTime;
            EndTime = endTime;
            ScheduleCron = scheduleCron;
            Permissions = permissions;
            DefaultRoomPermissions = defaultRoomPermissions;
            ModeratorPermissions = moderatorPermissions;
        }

        public string? Name { get; }

        public string ConferenceType { get; }

        public IImmutableList<string> Moderators { get; }

        public DateTimeOffset? StartTime { get; }

        public DateTimeOffset? EndTime { get; }

        public string? ScheduleCron { get; }

        public IImmutableDictionary<string, string>? Permissions { get; }

        public IImmutableDictionary<string, string>? DefaultRoomPermissions { get; }

        public IImmutableDictionary<string, string>? ModeratorPermissions { get; }
    }
}