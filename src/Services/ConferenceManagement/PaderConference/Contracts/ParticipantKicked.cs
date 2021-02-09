namespace PaderConference.Contracts
{
    public interface ParticipantKicked
    {
        string ParticipantId { get; }

        string ConferenceId { get; }

        string? ConnectionId { get; }
    }
}
