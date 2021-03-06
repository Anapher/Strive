namespace PaderConference.Models.Response
{
    public class ConferenceCreatedResponseDto
    {
        public ConferenceCreatedResponseDto(string conferenceId)
        {
            ConferenceId = conferenceId;
        }

        public string ConferenceId { get; set; }
    }
}