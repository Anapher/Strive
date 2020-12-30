using FluentValidation;
using PaderConference.Core.Services.Chat.Requests;

namespace PaderConference.Core.Services.Chat.Validation
{
    public class SendChatMessageRequestValidator : AbstractValidator<SendChatMessageRequest>
    {
        public SendChatMessageRequestValidator()
        {
            RuleFor(x => x.Message).NotEmpty();
        }
    }
}
