namespace PaderConference.Core.Services.Chat.Dto
{
    public class SendChatMessageDto
    {
        public string? Message { get; set; }

        public SendingMode? Mode { get; set; }
    }

    public abstract class SendingMode
    {
        public abstract string Type { get; }
    }

    public class SendAnonymously : SendingMode
    {
        public override string Type { get; } = "anonymously";
    }

    public class SendPrivately : SendingMode
    {
        public override string Type { get; } = "privately";

        public string? ToParticipant { get; set; }
    }
}