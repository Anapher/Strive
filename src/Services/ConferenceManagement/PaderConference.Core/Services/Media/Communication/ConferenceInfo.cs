namespace PaderConference.Core.Services.Media.Communication
{
    public class ConferenceInfo
    {
        public ConferenceInfo(string conferenceId)
        {
            Id = conferenceId;
        }

        public string Id { get; set; }
    }
}