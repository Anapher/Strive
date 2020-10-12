using StackExchange.Redis;

namespace PaderConference.Infrastructure.Services.Media.Communication
{
    public static class RedisCommunication
    {
        /// <summary>
        ///     The key where the new conferences (List of <see cref="ConferenceInfo" />) are stored
        /// </summary>
        public const string NewConferencesKey = "newConferences";

        /// <summary>
        ///     Published once a new conference was added to <see cref="NewConferencesKey" />
        /// </summary>
        public static readonly RedisChannel NewConferenceChannel =
            new RedisChannel("newConferenceCreated", RedisChannel.PatternMode.Literal);

        /// <summary>
        ///     Published once a new conference was added to <see cref="NewConferencesKey" />
        /// </summary>
        public static readonly ChannelName ClientDisconnectedChannel = CreateChannelName("::clientDisconnected");

        /// <summary>
        ///     The key where the rtc capabilities of the router are stored, encoded in JSON
        /// </summary>
        public static readonly ChannelName RtpCapabilitiesKey = CreateChannelName("::routerRtpCapabilities");

        /// <summary>
        ///     On send a message back to a connection
        /// </summary>
        public static readonly ChannelName OnSendMessageToConnection = CreateChannelName("::sendMessageToConnection");

        private static ChannelName CreateChannelName(string postFix)
        {
            return new ChannelName(postFix);
        }

        public static class Request
        {
            /// <summary>
            ///     Initialize a new connection, must be called before a transport can be created
            /// </summary>
            public static readonly ChannelName InitializeConnection = CreateChannelName("/req::initializeConnection");

            /// <summary>
            ///     Create a new transport
            /// </summary>
            public static readonly ChannelName CreateTransport = CreateChannelName("/req::createTransport");

            /// <summary>
            ///     Connect a transport
            /// </summary>
            public static readonly ChannelName ConnectTransport = CreateChannelName("/req::connectTransport");

            /// <summary>
            ///     On transport produce
            /// </summary>
            public static readonly ChannelName TransportProduce = CreateChannelName("/req::transportProduce");

            /// <summary>
            ///     On transport produce data
            /// </summary>
            public static readonly ChannelName TransportProduceData = CreateChannelName("/req::transportProduceData");
        }

        public class ChannelName
        {
            private readonly string _postFix;

            public ChannelName(string postFix)
            {
                _postFix = postFix;
            }

            public string GetName(string conferenceId)
            {
                return conferenceId + _postFix;
            }

            public bool Match(string s)
            {
                return s.EndsWith(_postFix);
            }
        }
    }
}