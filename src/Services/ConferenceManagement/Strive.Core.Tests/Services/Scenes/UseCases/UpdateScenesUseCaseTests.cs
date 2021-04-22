using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Moq.Language;
using Strive.Core.Domain.Entities;
using Strive.Core.Extensions;
using Strive.Core.Services.Scenes;
using Strive.Core.Services.Scenes.Gateways;
using Strive.Core.Services.Scenes.Requests;
using Strive.Core.Services.Scenes.Scenes;
using Strive.Core.Services.Scenes.UseCases;
using Strive.Core.Tests._TestHelpers;
using Xunit;

namespace Strive.Core.Tests.Services.Scenes.UseCases
{
    public class UpdateScenesUseCaseTests
    {
        private const string ConferenceId = "1";
        private const string RoomId = "2";

        private readonly Mock<IMediator> _mediator = new();
        private readonly Mock<ISceneRepository> _repository = new();

        private UpdateScenesUseCase Create(params ISceneProvider[] providers)
        {
            return new(_mediator.Object, _repository.Object, new[] {SetupSelfSceneProvider(AutonomousScene.Instance)}
                .Concat(providers));
        }

        private static ISceneProvider SetupSelfSceneProvider<T>(T scene) where T : IScene
        {
            var mock = new Mock<ISceneProvider>();
            mock.Setup(x => x.GetAvailableScenes(ConferenceId, RoomId, It.IsAny<IReadOnlyList<IScene>>()))
                .ReturnsAsync(new IScene[] {scene});
            mock.Setup(x => x.IsProvided(It.IsAny<T>())).Returns(true);
            mock.Setup(x => x.BuildStack(It.IsAny<T>(), It.IsAny<SceneBuilderContext>(), It.IsAny<SceneStackFunc>()))
                .ReturnsAsync(new IScene[] {scene});

            return mock.Object;
        }

        private void SetupSceneStackIsAvailable()
        {
            _mediator.Setup(x =>
                    x.Send(
                        It.Is<FetchAvailableScenesRequest>(x => x.ConferenceId == ConferenceId && x.RoomId == RoomId),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync((FetchAvailableScenesRequest request, CancellationToken _) => request.SceneStack);
        }

        private ISetupSequentialResult<Task<IReadOnlyList<IScene>>> SetupFetchAvailableSequence()
        {
            return _mediator.SetupSequence(x =>
                x.Send(It.Is<FetchAvailableScenesRequest>(x => x.ConferenceId == ConferenceId && x.RoomId == RoomId),
                    It.IsAny<CancellationToken>()));
        }

        private void SetupConference(Conference? conference = null)
        {
            var defaultConference = new Conference(ConferenceId);
            _mediator.SetupConference(conference ?? defaultConference);
        }

        private Func<SceneState> TrackSceneState(bool single = true)
        {
            SceneState? received = null;

            _repository.Setup(x => x.SetSceneState(ConferenceId, RoomId, It.IsAny<SceneState>())).Callback(
                (string _, string _, SceneState state) =>
                {
                    if (single)
                        Assert.Null(received);

                    received = state;
                });

            return () =>
            {
                Assert.NotNull(received);
                return received!;
            };
        }

        [Fact]
        public async Task Handle_NotSetInRepository_ApplyDefault()
        {
            SetupSceneStackIsAvailable();
            SetupConference();

            var stateCallback = TrackSceneState();

            // arrange
            var request = new UpdateScenesRequest(ConferenceId, RoomId);
            var useCase = Create();

            // act
            await useCase.Handle(request, CancellationToken.None);

            // assert
            var received = stateCallback();
            Assert.Equal(received.SceneStack, SynchronizedSceneProvider.GetDefaultScene().Yield());
            Assert.Equal(received.AvailableScenes, new[] {AutonomousScene.Instance});
        }

        [Fact]
        public async Task Handle_SelectedSceneSet_BuildStack()
        {
            SetupSceneStackIsAvailable();
            SetupConference();

            var testSceneProvider = SetupSelfSceneProvider(new TestScene());
            _repository.Setup(x => x.GetScene(ConferenceId, RoomId))
                .ReturnsAsync(new ActiveScene(new TestScene(), null));

            var stateCallback = TrackSceneState();

            // arrange
            var request = new UpdateScenesRequest(ConferenceId, RoomId);
            var useCase = Create(testSceneProvider);

            // act
            await useCase.Handle(request, CancellationToken.None);

            // assert
            var received = stateCallback();
            Assert.Equal(new TestScene().Yield(), received.SceneStack);
        }

        [Fact]
        public async Task Handle_SelectedSceneAndContentOverwriteSet_BuildStack()
        {
            SetupSceneStackIsAvailable();
            SetupConference();

            var testSceneProvider = SetupSelfSceneProvider(new TestScene());
            _repository.Setup(x => x.GetScene(ConferenceId, RoomId))
                .ReturnsAsync(new ActiveScene(new TestScene(), new AutonomousScene()));

            var stateCallback = TrackSceneState();

            // arrange
            var request = new UpdateScenesRequest(ConferenceId, RoomId);
            var useCase = Create(testSceneProvider);

            // act
            await useCase.Handle(request, CancellationToken.None);

            // assert
            var received = stateCallback();
            Assert.Equal(new IScene[] {new TestScene(), new AutonomousScene()}, received.SceneStack);
        }

        [Fact]
        public async Task Handle_RaceConditionSceneUnavailable_ApplyDefault()
        {
            SetupFetchAvailableSequence().ReturnsAsync(new IScene[] {new TestScene()})
                .ReturnsAsync(new IScene[] {new AutonomousScene()}).ReturnsAsync(new IScene[] {new AutonomousScene()})
                .ReturnsAsync(new IScene[] {new AutonomousScene()}).ReturnsAsync(new IScene[] {new AutonomousScene()})
                .ReturnsAsync(new IScene[] {new AutonomousScene()});

            SetupConference();

            var testSceneProvider = SetupSelfSceneProvider(new TestScene());
            _repository.Setup(x => x.GetScene(ConferenceId, RoomId))
                .ReturnsAsync(new ActiveScene(new TestScene(), null));

            var stateCallback = TrackSceneState(false);

            // arrange
            var request = new UpdateScenesRequest(ConferenceId, RoomId);
            var useCase = Create(testSceneProvider);

            // act
            await useCase.Handle(request, CancellationToken.None);

            // assert
            var received = stateCallback();
            Assert.Equal(new IScene[] {new AutonomousScene()}, received.SceneStack);
        }

        private record TestScene : IScene;
    }
}
