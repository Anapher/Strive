using System.Collections.Generic;
using FluentValidation;
using PaderConference.Core.Services.Rooms;

namespace PaderConference.Hubs.Validators
{
    public class RoomCreationInfosValidator : AbstractValidator<IReadOnlyList<RoomCreationInfo>>
    {
        public RoomCreationInfosValidator()
        {
            RuleFor(x => x).NotEmpty();
            RuleForEach(x => x)
                .ChildRules(creationInfoRules => creationInfoRules.RuleFor(x => x.DisplayName).NotEmpty());
        }
    }
}
