namespace PaderConference.Models.Response
{
    public class StartConferenceResponseDto
    {
        public StartConferenceResponseDto(string conferenceId)
        {
            ConferenceId = conferenceId;
        }

        public string ConferenceId { get; set; }
    }
}