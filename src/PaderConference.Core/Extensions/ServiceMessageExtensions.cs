using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Core.Dto;
using PaderConference.Core.Services;
using PaderConference.Core.Signaling;

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

        public static async Task ResponseError(this IServiceMessage serviceMessage, Error error)
        {
            await serviceMessage.SendToCallerAsync(CoreHubMessages.Response.OnError, error);
        }
    }
}