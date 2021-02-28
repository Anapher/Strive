using FluentValidation;
using PaderConference.Hubs.Dtos;

namespace PaderConference.Hubs.Validators
{
    public class SendChatMessageDtoValidator : AbstractValidator<SendChatMessageDto>
    {
        public SendChatMessageDtoValidator()
        {
            RuleFor(x => x.Message).NotEmpty();
        }
    }
}
