using FluentValidation;
using PaderConference.Hubs.Dtos;

namespace PaderConference.Hubs.Validators
{
    public class KickParticipantRequestDtoValidator : AbstractValidator<KickParticipantRequestDto>
    {
        public KickParticipantRequestDtoValidator()
        {
            RuleFor(x => x.ParticipantId).NotEmpty();
        }
    }
}