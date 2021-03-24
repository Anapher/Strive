using FluentValidation;
using PaderConference.Hubs.Core.Dtos;

namespace PaderConference.Hubs.Core.Validators
{
    public class KickParticipantRequestDtoValidator : AbstractValidator<KickParticipantRequestDto>
    {
        public KickParticipantRequestDtoValidator()
        {
            RuleFor(x => x.ParticipantId).NotEmpty();
        }
    }
}