using System;
using FluentValidation;
using PaderConference.Extensions;
using PaderConference.Models.Request;

namespace PaderConference.Models.Validation
{
    public class CreateConferenceRequestValidator : AbstractValidator<CreateConferenceRequestDto>
    {
        public CreateConferenceRequestValidator()
        {
            RuleFor(x => x.Organizers).NotEmpty();
            RuleFor(x => x.ScheduleCron).Must(x =>
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
        }
    }
}