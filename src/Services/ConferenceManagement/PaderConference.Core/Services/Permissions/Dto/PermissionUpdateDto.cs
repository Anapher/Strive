namespace PaderConference.Core.Services.Permissions.Dto
{
    public record PermissionUpdateDto
    {
        public string? ConferenceId { get; init; }
        public string? RoomId { get; init; }
        public string? ParticipantId { get; init; }
    }
}