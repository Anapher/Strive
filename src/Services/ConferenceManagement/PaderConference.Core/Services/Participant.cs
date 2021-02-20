namespace PaderConference.Core.Services
{
    public readonly struct Participant
    {
        public readonly string ConferenceId;
        public readonly string Id;

        public Participant(string conferenceId, string id)
        {
            ConferenceId = conferenceId;
            Id = id;
        }

        public void Deconstruct(out string conferenceId, out string participantId)
        {
            conferenceId = ConferenceId;
            participantId = Id;
        }

        public override string ToString()
        {
            return $"{Id} (joined to {ConferenceId})";
        }
    }
}
