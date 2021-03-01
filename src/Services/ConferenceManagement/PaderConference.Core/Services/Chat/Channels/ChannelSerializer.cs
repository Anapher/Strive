using System;
using System.Collections.Generic;
using System.Linq;
using PaderConference.Core.Services.Synchronization;

namespace PaderConference.Core.Services.Chat.Channels
{
    public static class ChannelSerializer
    {
        private const string PROP_TYPE = "type";

        public static SynchronizedObjectId Encode(ChatChannel channel)
        {
            var parameters = new Dictionary<string, string>();
            ChatChannelType type;

            switch (channel)
            {
                case GlobalChatChannel:
                    type = ChatChannelType.Global;
                    break;
                case PrivateChatChannel privateChatChannel:
                    type = ChatChannelType.Private;

                    var participants = privateChatChannel.Participants.ToList();
                    parameters.Add("p1", participants[0]);
                    parameters.Add("p2", participants[1]);
                    break;
                case RoomChatChannel roomChatChannel:
                    type = ChatChannelType.Room;
                    parameters.Add("roomId", roomChatChannel.RoomId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(channel));
            }

            return CreateSyncChannel(type, parameters);
        }

        public static ChatChannel Decode(SynchronizedObjectId channel)
        {
            var typeString = channel.Parameters[PROP_TYPE];
            var type = StringToChannelType(typeString);

            switch (type)
            {
                case ChatChannelType.Global:
                    return GlobalChatChannel.Instance;
                case ChatChannelType.Room:
                    return new RoomChatChannel(channel.Parameters["roomId"]);
                case ChatChannelType.Private:
                    return new PrivateChatChannel(new HashSet<string>
                    {
                        channel.Parameters["p1"], channel.Parameters["p2"],
                    });
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static SynchronizedObjectId CreateSyncChannel(ChatChannelType type,
            Dictionary<string, string> additionalParameters)
        {
            additionalParameters.Add(PROP_TYPE, ChannelTypeToString(type));
            return new SynchronizedObjectId(SynchronizedObjectIds.CHAT, additionalParameters);
        }

        private static string ChannelTypeToString(ChatChannelType type)
        {
            return type.ToString().ToLower();
        }

        private static ChatChannelType StringToChannelType(string s)
        {
            return Enum.Parse<ChatChannelType>(s, true);
        }
    }
}
