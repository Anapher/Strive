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
        ///     The key where the rtc capabilities of the router are stored, encoded in JSON
        /// </summary>
        public static ChannelName RtpCapabilitiesKey = CreateChannelName("::routerRtpCapabilities");

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
        }

        public static class Response
        {
            /// <summary>
            ///     Create a new transport
            /// </summary>
            public static readonly ChannelName CreateTransport = CreateChannelName("/res::createTransport");
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