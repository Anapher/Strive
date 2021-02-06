using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using PaderConference.Core.Services.Synchronization;
using PaderConference.Core.Services.Synchronization.Gateways;
using PaderConference.Core.Services.Synchronization.Notifications;
using PaderConference.Core.Services.Synchronization.Requests;
using PaderConference.Core.Services.Synchronization.UpdateStrategy;
using PaderConference.Core.Services.Synchronization.UseCases;
using PaderConference.Core.Tests._TestUtils;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.Core.Tests.Services.Synchronization.UseCases
{
    public class UpdateSynchronizedObjectHandlerTests
    {
        private readonly ILogger<UpdateSynchronizedObjectHandler<SyncObj>> _logger;

        public UpdateSynchronizedObjectHandlerTests(ITestOutputHelper output)
        {
            _logger = output.CreateLogger<UpdateSynchronizedObjectHandler<SyncObj>>();
        }

        [Fact]
        public Task Handle_ReplaceValue_ReturnNewValue()
        {
            return TestUpdateSyncObj(newValue => new ReplaceValueUpdate<SyncObj>(newValue));
        }

        [Fact]
        public Task Handle_PatchValue_ReturnNewValue()
        {
            return TestUpdateSyncObj(newValue => new PatchValueUpdate<SyncObj>(_ => newValue));
        }

        private async Task TestUpdateSyncObj(Func<SyncObj, IValueUpdate<SyncObj>> createValueUpdateFunc)
        {
            const string conferenceId = "test";
            const string syncObjName = "syncObj";
            const ParticipantGroup targetGroup = ParticipantGroup.All;

            var currentValue = new SyncObj {TestVal = false};
            var newValue = new SyncObj {TestVal = true};

            // arrange
            var mediatorMock = new Mock<IMediator>();
            var capturedNotification = mediatorMock.CaptureNotification<SynchronizedObjectUpdatedNotification>();

            var repositoryMock = new Mock<ISynchronizedObjectRepository>();
            repositoryMock.Setup(x => x.Update(conferenceId, syncObjName, It.IsAny<Func<SyncObj?, SyncObj>>()))
                .ReturnsAsync((currentValue, newValue));
            repositoryMock.Setup(x => x.Update(conferenceId, syncObjName, It.IsAny<SyncObj>()))
                .ReturnsAsync(currentValue);

            var handler =
                new UpdateSynchronizedObjectHandler<SyncObj>(repositoryMock.Object, mediatorMock.Object, _logger);

            var updateValue = createValueUpdateFunc(newValue);
            var meta = new SynchronizedObjectMetadata(conferenceId, syncObjName, targetGroup);

            var request = new UpdateSynchronizedObjectRequest<SyncObj>(updateValue, meta);

            // act
            var result = await handler.Handle(request, CancellationToken.None);

            // assert
            Assert.Equal(newValue, result);

            var notification = capturedNotification.GetNotification();
            Assert.Equal(conferenceId, notification.ConferenceId);
            Assert.Equal(syncObjName, notification.Name);
            Assert.Equal(currentValue, notification.PreviousValue);
            Assert.Equal(newValue, notification.Value);
        }

        private class SyncObj
        {
            public bool TestVal { get; set; }
        }
    }
}
