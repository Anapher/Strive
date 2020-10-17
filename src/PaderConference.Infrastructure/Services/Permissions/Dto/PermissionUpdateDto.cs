namespace PaderConference.Infrastructure.Services.Permissions.Dto
{
    public class PermissionUpdateDto
    {
        public PermissionUpdateDto(string? conferenceId, string? roomId, string? participantId)
        {
            ConferenceId = conferenceId;
            RoomId = roomId;
            ParticipantId = participantId;
        }

        public string? ConferenceId { get; }

        public string? RoomId { get; }

        public string? ParticipantId { get; }
    }
}