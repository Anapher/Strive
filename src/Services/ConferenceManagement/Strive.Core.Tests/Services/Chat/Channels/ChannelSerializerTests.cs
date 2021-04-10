using System.Collections.Generic;
using Strive.Core.Services.Chat.Channels;
using Strive.Core.Services.Synchronization;
using Xunit;

namespace Strive.Core.Tests.Services.Chat.Channels
{
    public class ChannelSerializerTests
    {
        public static readonly TheoryData<ChatChannel, SynchronizedObjectId> TestData = new()
        {
            {GlobalChatChannel.Instance, SynchronizedObjectId.Parse("chat?type=global")},
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

        [Fact]
        public void TestSortParameterKeys()
        {
            // arrange
            var syncObjId = new SynchronizedObjectId("test",
                new Dictionary<string, string> {{"c", "1"}, {"a", "5"}, {"b", "3"}});

            // act
            var result = syncObjId.ToString();

            // assert
            Assert.Equal("test?a=5&b=3&c=1", result);
        }
    }
}
