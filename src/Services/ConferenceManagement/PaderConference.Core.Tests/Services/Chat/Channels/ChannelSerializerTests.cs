using System.Collections.Generic;
using PaderConference.Core.Services.Chat.Channels;
using PaderConference.Core.Services.Synchronization;
using Xunit;

namespace PaderConference.Core.Tests.Services.Chat.Channels
{
    public class ChannelSerializerTests
    {
        public static readonly TheoryData<ChatChannel, SynchronizedObjectId> TestData = new()
        {
            {new GlobalChatChannel(), SynchronizedObjectId.Parse("chat?type=global")},
            {new RoomChatChannel("testId"), SynchronizedObjectId.Parse("chat?type=room&roomId=testId")},
            {
                new PrivateChatChannel(new HashSet<string> {"participantId1", "participantId2"}),
                SynchronizedObjectId.Parse("chat?type=private&p1=participantId1&p2=participantId2")
            },
        };

        [Theory]
        [MemberData(nameof(TestData))]
        public void Encode_TestValues(ChatChannel channel, SynchronizedObjectId expected)
        {
            var encoded = ChannelSerializer.Encode(channel);

            Assert.Equal(expected.Id, encoded.Id);
            Assert.Equal(expected.Parameters, encoded.Parameters);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void Decode_TestValues(ChatChannel expected, SynchronizedObjectId channelId)
        {
            var channel = ChannelSerializer.Decode(channelId);

            if (channel is PrivateChatChannel privateChannel)
                Assert.Equal(((PrivateChatChannel) expected).Participants, privateChannel.Participants);
            else
                Assert.Equal(expected, channel);
        }
    }
}
