using PaderConference.Core.Domain.Entities;
using PaderConference.Infrastructure.Services;
using PaderConference.Infrastructure.Services.Permissions.Dto;
using PaderConference.Infrastructure.Services.Rooms.Messages;
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
        ///     Invoked once a participant switched the room. The parameter is <see cref="ConnectionMessage{TPayload}" /> of
        ///     <see cref="RoomSwitchInfo" />
        /// </summary>
        public static string RoomSwitchedChannel(string conferenceId)
        {
            return $"{conferenceId}::roomSwitched";
        }

        public static class Media
        {
            /// <summary>
            ///     Published once a new conference was added to <see cref="RedisKeys.Media.NewConferencesKey" />
            /// </summary>
            public static readonly RedisChannel NewConferenceCreated = "newConferenceCreated";

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
            }
        }

        private static ConferenceDependentKey CreateChannelName(string postFix)
        {
            return new ConferenceDependentKey(postFix);
        }
    }

    public class ConferenceDependentKey
    {
        private readonly string _postFix;

        public ConferenceDependentKey(string postFix)
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
