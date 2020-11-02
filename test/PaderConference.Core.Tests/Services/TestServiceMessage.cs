using Moq;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Services;

namespace PaderConference.Core.Tests.Services
{
    public class TestServiceMessage
    {
        public static Mock<IServiceMessage<T>> Create<T>(T payload, Participant participant, string connectionId)
        {
            var mock = new Mock<IServiceMessage<T>>();
            mock.SetupGet(x => x.Participant).Returns(participant);
            mock.SetupGet(x => x.ConnectionId).Returns(connectionId);
            mock.SetupGet(x => x.Payload).Returns(payload);

            return mock;
        }

        public static Mock<IServiceMessage> Create(Participant participant, string connectionId)
        {
            var mock = new Mock<IServiceMessage>();
            mock.SetupGet(x => x.Participant).Returns(participant);
            mock.SetupGet(x => x.ConnectionId).Returns(connectionId);

            return mock;
        }
    }
}
