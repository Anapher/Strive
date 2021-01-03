namespace PaderConference.Core.Services.Chat.Dto
{
    public class SendPrivately : SendingMode
    {
        public const string TYPE = "privately";

        public override string Type { get; } = TYPE;

        public ParticipantRef? To { get; set; }
    }
}
