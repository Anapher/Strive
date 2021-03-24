using FluentValidation;
using PaderConference.Hubs.Core.Dtos;

namespace PaderConference.Hubs.Core.Validators
{
    public class SendChatMessageDtoValidator : AbstractValidator<SendChatMessageDto>
    {
        public SendChatMessageDtoValidator()
        {
            RuleFor(x => x.Message).NotEmpty();
        }
    }
}
