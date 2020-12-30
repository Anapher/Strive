namespace PaderConference.Core.Services.Chat.Dto
{
    public class SendPrivately : SendingMode
    {
        public override string Type { get; } = "privately";

        public ParticipantRef? To { get; set; }
    }
}
