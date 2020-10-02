namespace PaderConference.Core.Dto.UseCaseResponses
{
    public class StartConferenceResponse
    {
        public StartConferenceResponse(string conferenceId)
        {
            ConferenceId = conferenceId;
        }

        public string ConferenceId { get; }
    }
}