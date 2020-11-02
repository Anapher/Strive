using System;
using PaderConference.Core.Services.Chat.Dto;

namespace PaderConference.Core.Services.Chat.Filters
{
    public class FilterFactory
    {
        public static IMessageFilter CreateFilter(SendingMode? mode, string senderConnectionId)
        {
            if (mode is SendPrivately sendPrivately)
            {
                if (sendPrivately.ToParticipant == null)
                    throw new ArgumentException(
                        $"{nameof(SendPrivately)}.{nameof(SendPrivately.ToParticipant)} must not be null");

                return new IncludeFilter(new[] {sendPrivately.ToParticipant}, senderConnectionId);
            }

            return new AtAllFilter();
        }
    }
}