using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Services;
using PaderConference.Core.Services.Media.Communication;
using PaderConference.Core.Services.Permissions.Dto;
using StackExchange.Redis;

namespace PaderConference.Infrastructure
{
    public static class RedisChannels
    {
        /// <summary>
        ///     Reset all conferences, close existing ones and close all connections. No parameter
        /// </summary>
        public const string OnResetConferences = "resetConferences";

        /// <summary>
        ///     When the permissions updated (of a participant, conference, ...). The parameter is a
        ///     <see cref="PermissionUpdateDto" />
        /// </summary>
        public const string OnPermissionsUpdated = "permissionsUpdated";

        /// <summary>
        ///     Invoked once a conference was updated. The parameter is a <see cref="Conference" />
        /// </summary>
        /// <param name="conferenceId">The conference id</param>
        public static string OnConferenceUpdated(string conferenceId) => $"conferenceUpdated::{conferenceId}";

        /// <summary>
        ///     Invoked once a participant switched the room.
        /// </summary>
        public static string RoomSwitchedChannel(string conferenceId)
        {
            return $"{conferenceId}::roomSwitched";
        }

        /// <summary>
        ///     Published once a client disconnected
        /// </summary>
        public static readonly ConferenceDependentKey ClientDisconnectedChannel =
            CreateChannelName("::clientDisconnected");

        /// <summary>
        ///     On send a message back to a connection
        /// </summary>
        public static readonly ConferenceDependentKey OnSendMessageToConnection =
            CreateChannelName("::sendMessageToConnection");

        public static class Media
        {
            /// <summary>
            ///     Published once a new conference was added to <see cref="RedisKeys.Media.NewConferencesKey" />
            /// </summary>
            public static readonly RedisChannel NewConferenceCreated = "newConferenceCreated";

            /// <summary>
            ///     Published in a given interval to notify about participant volumes. Parameter is an array
            /// </summary>
            public static readonly ConferenceDependentKey AudioObserver = CreateChannelName("::audioObserver");

            /// <summary>
            ///     The streams changed. Can be retrieved from <see cref="RedisKeys.Media.Streams" />
            /// </summary>
            public static readonly ConferenceDependentKey StreamsChanged = CreateChannelName("::streamsChanged");

            public static class Request
            {
                /// <summary>
                ///     Initialize a new connection, must be called before a transport can be created
                /// </summary>
                public static readonly ConferenceDependentKey InitializeConnection =
                    CreateChannelName("/req::initializeConnection");

                /// <summary>
                ///     Create a new transport
                /// </summary>
                public static readonly ConferenceDependentKey CreateTransport =
                    CreateChannelName("/req::createTransport");

                /// <summary>
                ///     Connect a transport
                /// </summary>
                public static readonly ConferenceDependentKey ConnectTransport =
                    CreateChannelName("/req::connectTransport");

                /// <summary>
                ///     On transport produce
                /// </summary>
                public static readonly ConferenceDependentKey TransportProduce =
                    CreateChannelName("/req::transportProduce");

                /// <summary>
                ///     Change stream. Parameter is <see cref="ChangeStreamDto" />
                /// </summary>
                public static readonly ConferenceDependentKey ChangeStream = CreateChannelName("/req::changeStream");
            }
        }

        private static ConferenceDependentKey CreateChannelName(string postFix)
        {
            return new ConferenceDependentKey(postFix);
        }
    }
}
