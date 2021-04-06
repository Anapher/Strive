using System.Collections.Generic;
using FluentValidation;
using Strive.Core.Services.Rooms;

namespace Strive.Hubs.Core.Validators
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
