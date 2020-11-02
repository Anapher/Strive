using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using PaderConference.Core.Services;
using PaderConference.Core.Services.Media.Communication;
using PaderConference.Core.Services.Media.Mediasoup;

namespace PaderConference.Core.Interfaces.Gateways.Repositories
{
    public interface IMediaRepo
    {
        Task RegisterConference(string conferenceId);

        Task<Func<Task>> SubscribeOnSendMessage(string conferenceId, Func<SendToConnectionDto, Task> handler);

        Task<Func<Task>> SubscribeStreamsChanged(string conferenceId, Func<Task> handler);

        Task<Dictionary<string, ParticipantStreamInfo>> GetStreams(string conferenceId);

        Task<JsonElement?> GetRtpCapabilities(string conferenceId);

        Task NotifyClientDisconnected(ConnectionMessageMetadata meta);

        Task<JsonElement?> SendMessage<TRequest>(ConferenceDependentKey key, string conferenceId,
            ConnectionMessage<TRequest> message);
    }
}
