using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Services.Media;
using PaderConference.Core.Services.Media.Dtos;
using PaderConference.Hubs.Core;
using PaderConference.IntegrationTests._Helpers;
using PaderConference.Messaging.SFU.Dto;
using PaderConference.Messaging.SFU.SendContracts;
using PaderConference.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.IntegrationTests.Services
{
    [Collection(IntegrationTestCollection.Definition)]
    public class MediaTests : ServiceIntegrationTest
    {
        public MediaTests(ITestOutputHelper testOutputHelper, MongoDbFixture mongoDb) : base(testOutputHelper, mongoDb)
        {
        }

        private Mock<IPublishObserver> ConnectPublishObserver()
        {
            var mockObserver = new Mock<IPublishObserver>();
            var busControl = Factory.Services.GetRequiredService<IBusControl>();
            busControl.ConnectPublishObserver(mockObserver.Object);

            return mockObserver;
        }

        [Fact]
        public async Task FetchSfuConnectionInfo_ClosedConference_ReturnInfo()
        {
            // arrange
            var conference = await CreateConference();
            var connection = await ConnectUserToConference(Moderator, conference);

            // act
            var result =
                await connection.Hub.InvokeAsync<SuccessOrError<SfuConnectionInfo>>(
                    nameof(CoreHub.FetchSfuConnectionInfo));

            // assert
            AssertSuccess(result);
            Assert.NotNull(result.Response!.AuthToken);
            Assert.NotNull(result.Response!.Url);
        }

        [Fact]
        public async Task ChangeParticipantProducer_SendMessageToSfu()
        {
            // arrange
            var conference = await CreateConference(Moderator);
            var connection = await ConnectUserToConference(Moderator, conference);

            var request = new ChangeParticipantProducerDto("123", ProducerSource.Mic, MediaStreamAction.Pause);

            var observer = ConnectPublishObserver();
            observer.Setup(x => x.PostPublish(It.IsAny<PublishContext<ChangeParticipantProducer>>())).Callback(
                (PublishContext<ChangeParticipantProducer> context) =>
                {
                    var dto = context.Message;
                    Assert.Equal(conference.ConferenceId, dto.ConferenceId);
                    Assert.Equal(request.ParticipantId, dto.Payload.ParticipantId);
                    Assert.Equal(request.Source, dto.Payload.Source);
                    Assert.Equal(request.Action, dto.Payload.Action);
                });

            // act
            var result =
                await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.ChangeParticipantProducer),
                    request);

            // assert
            AssertSuccess(result);

            await AssertHelper.WaitForAssert(() =>
            {
                observer.Verify(x => x.PostPublish(It.IsAny<PublishContext<ChangeParticipantProducer>>()),
                    Times.Once);
            });
        }
    }
}
