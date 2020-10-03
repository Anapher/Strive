using System;

namespace PaderConference.Hubs.Chat.Filters
{
    public class FilterFactory
    {
        public static IMessageFilter CreateFilter(ChatMessageFilter? filter, string senderConnectionId)
        {
            if (filter == null)
                return new AtAllFilter();

            if (filter.Exclude != null)
                return new ExcludeFilter(filter.Exclude);

            if (filter.Include != null)
                return new IncludeFilter(filter.Include, senderConnectionId);

            throw new InvalidOperationException("Message has no addressees");
        }
    }
}