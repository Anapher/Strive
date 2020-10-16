namespace PaderConference.Core.Dto.UseCaseResponses
{
    public class CreateConferenceResponse
    {
        public CreateConferenceResponse(string conferenceId)
        {
            ConferenceId = conferenceId;
        }

        public string ConferenceId { get; }
    }
}