namespace PaderConference.Core.Services.Chat.Dto
{
    public class SendAnonymously : SendingMode
    {
        public const string TYPE = "anonymously";

        public override string Type { get; } = TYPE;
    }
}
