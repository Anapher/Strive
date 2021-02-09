using FluentValidation;
using PaderConference.Core.Services.ConferenceControl.Requests;

namespace PaderConference.Hubs.Validators
{
    public class KickParticipantRequestValidator : AbstractValidator<KickParticipantRequest>
    {
        public KickParticipantRequestValidator()
        {
            RuleFor(x => x.ParticipantId).NotEmpty();
        }
    }
}
