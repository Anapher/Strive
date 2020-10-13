using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using PaderConference.Core.Dto;
using PaderConference.Infrastructure.Services;

namespace PaderConference.Infrastructure.Extensions
{
    public static class ServiceMessageExtensions
    {
        public static object GetScopeData(this IServiceMessage serviceMessage)
        {
            return new Dictionary<string, object>
            {
                {"connectionId", serviceMessage.Context.ConnectionId},
                {"participantId", serviceMessage.Participant.ParticipantId}
            };
        }

        public static async Task ResponseError(this IServiceMessage serviceMessage, Error error)
        {
            await serviceMessage.Clients.Caller.SendAsync("OnError", error);
        }
    }
}