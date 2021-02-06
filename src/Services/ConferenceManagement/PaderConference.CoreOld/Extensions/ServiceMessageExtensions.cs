using System.Collections.Generic;
using PaderConference.Core.Services;

namespace PaderConference.Core.Extensions
{
    public static class ServiceMessageExtensions
    {
        public static object GetScopeData(this IServiceMessage serviceMessage)
        {
            return new Dictionary<string, object>
            {
                {"connectionId", serviceMessage.ConnectionId},
                {"participantId", serviceMessage.Participant.ParticipantId},
            };
        }
    }
}