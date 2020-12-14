using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Moq;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Services.Synchronization;
using PaderConference.Core.Signaling;
using PaderConference.Infrastructure.Sockets;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.Core.Tests.Services.Synchronization
{
    public class SynchronizationServiceTests : ServiceTest<SynchronizationService>
    {
        private record TestSyncObj(string Test1);

        private const string ConferenceId = "conid";
        private readonly Mock<ISignalMessenger> _signalMessenger = new();
        private readonly Mock<IConferenceRepo> _conferenceRepo = new();
        private readonly Mock<IConferenceManager> _conferenceManager = new();
        private readonly Mock<IConnectionMapping> _connectionMapping = new();

        private readonly ConcurrentDictionary<string, IParticipantConnections> _connections = new();

        public SynchronizationServiceTests(ITestOutputHelper output) : base(output)
        {
            _conferenceRepo.Setup(x => x.FindById(ConferenceId))
                .ReturnsAsync(new Conference(ConferenceId, ImmutableList<string>.Empty));

            _connectionMapping.SetupGet(x => x.ConnectionsR).Returns(_connections);
        }

        private SynchronizationService Create()
        {
            return new(_signalMessenger.Object, ConferenceId, _conferenceRepo.Object, _conferenceManager.Object,
                _connectionMapping.Object, Logger);
        }

        private Participant AddParticipant(string id, string connectionId)
        {
            var participant = new Participant(id, default, "", default);

            Assert.True(_connections.TryAdd(id, new ParticipantConnections(connectionId)));
            _conferenceManager.Setup(x => x.TryGetParticipant(ConferenceId, id, out participant)).Returns(true);

            return participant;
        }

        [Fact]
        public async Task TestCreateSynchronizedObject()
        {
            // arrange
            var service = Create();

            // act
            await service.InitializeAsync();
            var synchronizedObj = service.Register("test", new TestSyncObj(string.Empty), ParticipantGroup.All);

            // assert
            Assert.NotNull(synchronizedObj);
        }

        [Fact]
        public async Task TestCreateSynchronizedObjectTwice()
        {
            // arrange
            var service = Create();

            // act
            await service.InitializeAsync();
            service.Register("test", new TestSyncObj(string.Empty), ParticipantGroup.All);

            Assert.ThrowsAny<Exception>(() => service.Register("test", new TestSyncObj("test"), ParticipantGroup.All));
        }

        [Fact]
        public async Task TestInitializeParticipant()
        {
            const string participantId = "id1";
            const string connectionId = "id1";

            // arrange
            var service = Create();
            var participant = AddParticipant(participantId, connectionId);
            var initialObj = new TestSyncObj("hello world");

            // act
            await service.InitializeAsync();

            service.Register("test", initialObj, ParticipantGroup.All);

            await service.InitializeParticipant(participant);

            // assert
            _signalMessenger.Verify(
                x => x.SendToConnectionAsync(connectionId, CoreHubMessages.Response.OnSynchronizeObjectState,
                    It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task TestUpdateStatusForParticipant()
        {
            const string participantId = "id1";
            const string connectionId = "id1";

            // arrange
            var service = Create();
            var participant = AddParticipant(participantId, connectionId);

            // act
            await service.InitializeAsync();
            var initialObj = new TestSyncObj("hello world");

            var syncObj = service.Register("test", initialObj, ParticipantGroup.All);

            await service.InitializeParticipant(participant);

            await syncObj.Update(initialObj with {Test1 = "test"});

            // assert
            _signalMessenger.Verify(
                x => x.SendToConferenceAsync(ConferenceId, CoreHubMessages.Response.OnSynchronizedObjectUpdated,
                    It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task TestUpdateStatusWithoutChangingAnythingForParticipant()
        {
            const string participantId = "id1";
            const string connectionId = "id1";

            // arrange
            var service = Create();
            var participant = AddParticipant(participantId, connectionId);

            // act
            await service.InitializeAsync();
            var initialObj = new TestSyncObj("hello world");

            var syncObj = service.Register("test", initialObj, ParticipantGroup.All);

            await service.InitializeParticipant(participant);

            await syncObj.Update(initialObj with { Test1 = "hello world" });

            // assert
            _signalMessenger.Verify(
                x => x.SendToConferenceAsync(ConferenceId, CoreHubMessages.Response.OnSynchronizedObjectUpdated,
                    It.IsAny<object>()), Times.Never);
        }

        [Fact]
        public async Task TestInitializeStateForModerators()
        {
            const string plebParticipantId = "id1";
            const string modParticipantId = "id2";
            const string plebConnId = "connId1";
            const string modConnId = "connId2";

            // arrange
            var service = Create();
            var pleb = AddParticipant(plebParticipantId, plebConnId);
            var mod = AddParticipant(modParticipantId, modConnId);

            _conferenceRepo.Setup(x => x.FindById(ConferenceId))
                .ReturnsAsync(new Conference(ConferenceId, new[] {modParticipantId}.ToImmutableList()));

            // act
            await service.InitializeAsync();
            var initialObj = new TestSyncObj("hello world");

            var syncObj = service.Register("test", initialObj, ParticipantGroup.Moderators);

            await service.InitializeParticipant(pleb);
            await service.InitializeParticipant(mod);

            // assert
            _signalMessenger.Verify(
                x => x.SendToConnectionAsync(plebConnId, CoreHubMessages.Response.OnSynchronizeObjectState,
                    It.Is<IReadOnlyDictionary<string, object>>(x => x.Count == 0)), Times.Once);
            _signalMessenger.Verify(
                x => x.SendToConnectionAsync(modConnId, CoreHubMessages.Response.OnSynchronizeObjectState,
                    It.Is<IReadOnlyDictionary<string, object>>(x => x.Count == 1)), Times.Once);
        }

        [Fact]
        public async Task TestUpdateStateForModerators()
        {
            const string plebParticipantId = "id1";
            const string modParticipantId = "id2";
            const string plebConnId = "connId1";
            const string modConnId = "connId2";

            // arrange
            var service = Create();
            var pleb = AddParticipant(plebParticipantId, plebConnId);
            var mod = AddParticipant(modParticipantId, modConnId);

            _conferenceRepo.Setup(x => x.FindById(ConferenceId))
                .ReturnsAsync(new Conference(ConferenceId, new[] {modParticipantId}.ToImmutableList()));

            // act
            await service.InitializeAsync();
            var initialObj = new TestSyncObj("hello world");

            var syncObj = service.Register("test", initialObj, ParticipantGroup.Moderators);

            await service.InitializeParticipant(pleb);
            await service.InitializeParticipant(mod);

            await syncObj.Update(initialObj with {Test1 = "2"});

            // assert
            _signalMessenger.Verify(x => x.SendToConferenceAsync(ConferenceId, It.IsAny<string>(), It.IsAny<object>()),
                Times.Never);
            _signalMessenger.Verify(
                x => x.SendToConnectionAsync(plebConnId, CoreHubMessages.Response.OnSynchronizedObjectUpdated,
                    It.IsAny<object>()), Times.Never);
            _signalMessenger.Verify(
                x => x.SendToConnectionAsync(modConnId, CoreHubMessages.Response.OnSynchronizedObjectUpdated,
                    It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task TestAddModerator()
        {
            const string plebParticipantId = "id1";
            const string modParticipantId = "id2";
            const string plebConnId = "connId1";
            const string modConnId = "connId2";

            // arrange
            var service = Create();
            var pleb = AddParticipant(plebParticipantId, plebConnId);
            var mod = AddParticipant(modParticipantId, modConnId);

            _conferenceRepo.Setup(x => x.FindById(ConferenceId))
                .ReturnsAsync(new Conference(ConferenceId, new[] {modParticipantId}.ToImmutableList()));

            Func<Conference, Task>? onUpdateHandler = null;

            _conferenceRepo.Setup(x => x.SubscribeConferenceUpdated(ConferenceId, It.IsAny<Func<Conference, Task>>()))
                .Callback((string _, Func<Conference, Task> handler) => onUpdateHandler = handler);

            // act
            await service.InitializeAsync();
            var initialObj = new TestSyncObj("hello world");

            service.Register("test", initialObj, ParticipantGroup.Moderators);

            await service.InitializeParticipant(pleb);
            await service.InitializeParticipant(mod);

            _signalMessenger.Reset();

            await onUpdateHandler.Invoke(new Conference(ConferenceId,
                new[] {modParticipantId, plebParticipantId}.ToImmutableList()));

            // assert
            _signalMessenger.Verify(
                x => x.SendToConnectionAsync(plebConnId, CoreHubMessages.Response.OnSynchronizeObjectState,
                    It.Is<IReadOnlyDictionary<string, object>>(x => x.Count == 1)), Times.Once);

            _signalMessenger.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task TestRemoveModerator()
        {
            const string plebParticipantId = "id1";
            const string modParticipantId = "id2";
            const string plebConnId = "connId1";
            const string modConnId = "connId2";

            // arrange
            var service = Create();
            var pleb = AddParticipant(plebParticipantId, plebConnId);
            var mod = AddParticipant(modParticipantId, modConnId);

            _conferenceRepo.Setup(x => x.FindById(ConferenceId))
                .ReturnsAsync(new Conference(ConferenceId, new[] {modParticipantId}.ToImmutableList()));

            Func<Conference, Task>? onUpdateHandler = null;

            _conferenceRepo.Setup(x => x.SubscribeConferenceUpdated(ConferenceId, It.IsAny<Func<Conference, Task>>()))
                .Callback((string _, Func<Conference, Task> handler) => onUpdateHandler = handler);

            // act
            await service.InitializeAsync();
            var initialObj = new TestSyncObj("hello world");

            service.Register("test", initialObj, ParticipantGroup.Moderators);

            await service.InitializeParticipant(pleb);
            await service.InitializeParticipant(mod);

            _signalMessenger.Reset();

            await onUpdateHandler.Invoke(new Conference(ConferenceId, ImmutableList<string>.Empty));

            // assert
            _signalMessenger.Verify(
                x => x.SendToConnectionAsync(modConnId, CoreHubMessages.Response.OnSynchronizeObjectState,
                    It.Is<IReadOnlyDictionary<string, object>>(x => x.Count == 0)), Times.Once);

            _signalMessenger.VerifyNoOtherCalls();
        }
    }
}
