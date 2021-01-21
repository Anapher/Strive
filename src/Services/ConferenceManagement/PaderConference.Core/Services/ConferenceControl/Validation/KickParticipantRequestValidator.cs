using FluentValidation;
using PaderConference.Core.Services.ConferenceControl.Requests;

namespace PaderConference.Core.Services.ConferenceControl.Validation
{
    public class KickParticipantRequestValidator : AbstractValidator<KickParticipantRequest>
    {
        public KickParticipantRequestValidator()
        {
            RuleFor(x => x.ParticipantId).NotEmpty();
        }
    }
}
