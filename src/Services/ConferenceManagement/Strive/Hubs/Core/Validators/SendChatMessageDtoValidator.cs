using FluentValidation;
using Strive.Hubs.Core.Dtos;

namespace Strive.Hubs.Core.Validators
{
    public class SendChatMessageDtoValidator : AbstractValidator<SendChatMessageDto>
    {
        public SendChatMessageDtoValidator()
        {
            RuleFor(x => x.Message).NotEmpty();
        }
    }
}
