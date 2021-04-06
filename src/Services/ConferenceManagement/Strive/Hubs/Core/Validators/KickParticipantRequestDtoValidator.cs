using FluentValidation;
using Strive.Hubs.Core.Dtos;

namespace Strive.Hubs.Core.Validators
{
    public class KickParticipantRequestDtoValidator : AbstractValidator<KickParticipantRequestDto>
    {
        public KickParticipantRequestDtoValidator()
        {
            RuleFor(x => x.ParticipantId).NotEmpty();
        }
    }
}