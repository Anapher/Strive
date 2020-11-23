using System;
using PaderConference.Core.Services.Chat.Dto;

namespace PaderConference.Core.Services.Chat.Filters
{
    public class FilterFactory
    {
        public static IMessageFilter CreateFilter(SendingMode? mode, string senderParticipantId)
        {
            if (mode is SendPrivately sendPrivately)
            {
                if (sendPrivately.To?.ParticipantId == null)
                    throw new ArgumentException($"{nameof(SendPrivately)}.{nameof(SendPrivately.To)} must not be null");

                return new IncludeFilter(new[] {sendPrivately.To.ParticipantId}, senderParticipantId);
            }

            return new AtAllFilter();
        }
    }
}