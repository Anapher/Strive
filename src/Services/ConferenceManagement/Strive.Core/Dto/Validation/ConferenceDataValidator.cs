using System;
using FluentValidation;
using Strive.Core.Dto.Services;
using Strive.Core.Extensions;
using Strive.Core.Services.Permissions;

namespace Strive.Core.Dto.Validation
{
    public class ConferenceDataValidator : AbstractValidator<ConferenceData>
    {
        public ConferenceDataValidator()
        {
            RuleFor(x => x.Configuration).NotNull();
            RuleFor(x => x.Configuration.Moderators).NotEmpty();
            RuleFor(x => x.Configuration.ScheduleCron).Must(x =>
            {
                if (x == null) return true;
                try
                {
                    CronYearParser.GetNextOccurrence(x, DateTimeOffset.UtcNow, TimeZoneInfo.Utc);
                }
                catch (Exception)
                {
                    return false;
                }

                return true;
            });

            RuleFor(x => x.Configuration.Chat.CancelParticipantIsTypingAfter).GreaterThanOrEqualTo(1);

            RuleForEach(x => x.Permissions).ChildRules(group =>
            {
                group.RuleFor(x => x.Key).IsInEnum();
                group.RuleForEach(x => x.Value).Must(x => DefinedPermissionsProvider.All.ContainsKey(x.Key))
                    .WithMessage(x => $"The permission key {x.Key} was not found.")
                    .Must(x => DefinedPermissionsProvider.All.TryGetValue(x.Key, out var descriptor) &&
                               descriptor.ValidateValue(x.Value)).WithMessage(x =>
                        $"The value of permission key {x.Key} doesn't match value type.");
            });
        }
    }
}
