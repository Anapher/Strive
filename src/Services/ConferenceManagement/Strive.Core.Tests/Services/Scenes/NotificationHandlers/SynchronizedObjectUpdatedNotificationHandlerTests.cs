using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Strive.Core.Extensions;
using Strive.Core.Services;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Scenes;
using Strive.Core.Services.Scenes.Gateways;
using Strive.Core.Services.Scenes.NotificationHandlers;
using Strive.Core.Services.Synchronization.Notifications;
using Strive.Core.Services.Synchronization.Requests;
using Xunit;

namespace Strive.Core.Tests.Services.Scenes.NotificationHandlers
{
    public class SynchronizedObjectUpdatedNotificationHandlerTests
    {
        private readonly Mock<IMediator> _mediator = new();
        private readonly Mock<ISceneProvider> _sceneProvider = new();
        private readonly Mock<ISceneRepository> _repository = new();

        private const string ConferenceId = "123";
        private const string RoomId = "45";

        private SynchronizedObjectUpdatedNotificationHandler Create()
        {
            return new(_mediator.Object, _sceneProvider.Object.Yield(), _repository.Object);
        }

        private void SetupRooms(params Room[] rooms)
        {
            var syncObj = new SynchronizedRooms(rooms, "default", ImmutableDictionary<string, string>.Empty);

            _mediator.Setup(x => x.Send(It.Is<FetchSynchronizedObjectRequest>(req => req.ConferenceId == ConferenceId),
                It.IsAny<CancellationToken>())).ReturnsAsync(syncObj);
        }

        [Fact]
        public async Task Handle_NoUpdatesRequired_DoNothing()
        {
            // arrange
            var useCase = Create();
            SetupRooms(new Room(RoomId, ""));

            _sceneProvider.Setup(x => x.IsUpdateRequired(ConferenceId, RoomId, It.IsAny<object>(), It.IsAny<object>()))
                .ReturnsAsync(SceneUpdate.NotRequired);

            // act
            var participants = new List<Participant> {new(ConferenceId, "1")};
            await useCase.Handle(new SynchronizedObjectUpdatedNotification(participants, "123", "hello", "hllo"),
                CancellationToken.None);

            // assert
            _repository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_UpdateRequiredButSceneStateNull_DoNothing()
        {
            // arrange
            var useCase = Create();
            SetupRooms(new Room(RoomId, ""));

            _sceneProvider.Setup(x => x.IsUpdateRequired(ConferenceId, RoomId, It.IsAny<object>(), It.IsAny<object>()))
                .ReturnsAsync(SceneUpdate.AvailableScenesChanged);

            // act
            var participants = new List<Participant> {new(ConferenceId, "1")};
            await useCase.Handle(new SynchronizedObjectUpdatedNotification(participants, "123", "hello", "hllo"),
                CancellationToken.None);

            // assert
            _repository.Verify(x => x.SetSceneState(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SceneState>()),
                Times.Never);
        }
    }
}
