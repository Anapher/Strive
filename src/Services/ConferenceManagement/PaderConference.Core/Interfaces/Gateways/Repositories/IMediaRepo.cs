using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
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

        Task<JObject?> GetRtpCapabilities(string conferenceId);

        Task NotifyClientDisconnected(ConnectionMessageMetadata meta);

        Task<SuccessOrError<JToken?>> SendMessage<TRequest>(ConferenceDependentKey key, string conferenceId,
            ConnectionMessage<TRequest> message);
    }
}
