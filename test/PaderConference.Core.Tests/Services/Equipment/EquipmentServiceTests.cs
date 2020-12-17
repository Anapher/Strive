using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Services.Equipment;
using PaderConference.Core.Services.Equipment.Data;
using PaderConference.Core.Services.Equipment.Dto;
using PaderConference.Core.Services.Media.Mediasoup;
using PaderConference.Core.Signaling;
using PaderConference.Infrastructure.Sockets;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.Core.Tests.Services.Equipment
{
    public class EquipmentServiceTests : ServiceTest<EquipmentService>
    {
        private const string TestToken = "test_token";
        private readonly Mock<ITokenFactory> _tokenFactory = new Mock<ITokenFactory>();
        private readonly Mock<ISignalMessenger> _signal = new Mock<ISignalMessenger>();
        private readonly ConnectionMapping _connectionMapping = new ConnectionMapping();
        private const string ConnectionId = "conn";
        private const string EqConnectionId = "conn2";

        public EquipmentServiceTests(ITestOutputHelper output) : base(output)
        {
        }

        private EquipmentService Create()
        {
            _connectionMapping.Add(ConnectionId, TestParticipants.Default, false);
            _tokenFactory.Setup(x => x.GenerateToken(It.IsAny<int>())).Returns(TestToken);

            return new EquipmentService(_tokenFactory.Object, _signal.Object, _connectionMapping, Logger);
        }

        [Fact]
        public async Task TestAuthenticateEquipmentNotExisting()
        {
            // arrange
            var service = Create();

            // act
            var response = await service.AuthenticateEquipment("token");

            // assert
            Assert.False(response.Success);
        }

        [Fact]
        public async Task TestGetTokenAndAuthenticate()
        {
            // arrange
            var service = Create();
            var message = TestServiceMessage.Create(TestParticipants.Default, ConnectionId).Object;

            // act
            var token = AssertSuccess(await service.GetEquipmentToken(message));
            var participantId = AssertSuccess(await service.AuthenticateEquipment(token));

            // assert
            Assert.Equal(TestToken, token);
            Assert.Equal(TestParticipants.Default.ParticipantId, participantId);
        }

        [Fact]
        public async Task<EquipmentService> TestEquipmentConnected()
        {
            // arrange
            var service = Create();
            var getTokenMessage = TestServiceMessage.Create(TestParticipants.Default, ConnectionId).Object;
            _signal.Setup(x =>
                    x.SendToConnectionAsync(ConnectionId, CoreHubMessages.Response.OnEquipmentUpdated,
                        It.IsAny<object>()))
                .Callback<string, string, object>((_, __, param) =>
                {
                    var status = Assert.IsType<List<ConnectedEquipmentDto>>(param);
                    Assert.NotEmpty(status);
                });

            // act
            var token = AssertSuccess(await service.GetEquipmentToken(getTokenMessage));
            AssertSuccess(await service.AuthenticateEquipment(token));

            await service.OnEquipmentConnected(TestParticipants.Default, EqConnectionId);

            // assert
            _signal.Verify(
                x => x.SendToConnectionAsync("conn", CoreHubMessages.Response.OnEquipmentUpdated, It.IsAny<object>()),
                Times.Once);
            _signal.VerifyNoOtherCalls();


            return service;
        }

        [Fact]
        public async Task TestRegisterEquipment()
        {
            var service = await TestEquipmentConnected();

            _signal.Reset();
            _signal.Setup(x =>
                    x.SendToConnectionAsync(ConnectionId, CoreHubMessages.Response.OnEquipmentUpdated,
                        It.IsAny<object>()))
                .Callback<string, string, object>((_, __, param) =>
                {
                    var status = Assert.IsType<List<ConnectedEquipmentDto>>(param);
                    var connected = Assert.Single(status);
                    Assert.Equal("Unit test", connected.Name);
                    Assert.NotEqual(Guid.Empty, connected.EquipmentId);
                    Assert.Null(connected.Status);

                    var device = Assert.Single(connected.Devices);
                    Assert.Equal("test", device.DeviceId);
                    Assert.Equal("Webcam", device.Label);
                    Assert.Equal(ProducerSource.Webcam, device.Source);
                });

            // arrange
            var registerMessage = TestServiceMessage.Create(
                new RegisterEquipmentRequestDto
                {
                    Name = "Unit test",
                    Devices = new List<EquipmentDeviceInfo>
                    {
                        new EquipmentDeviceInfo
                        {
                            DeviceId = "test", Label = "Webcam", Source = ProducerSource.Webcam,
                        },
                    },
                }, TestParticipants.Default, EqConnectionId).Object;

            // act
            await service.RegisterEquipment(registerMessage);

            // assert
            _signal.Verify(
                x => x.SendToConnectionAsync(ConnectionId, CoreHubMessages.Response.OnEquipmentUpdated,
                    It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task TestOnDisconnected()
        {
            var service = await TestEquipmentConnected();

            _signal.Reset();
            _signal.Setup(x =>
                    x.SendToConnectionAsync(ConnectionId, CoreHubMessages.Response.OnEquipmentUpdated,
                        It.IsAny<object>()))
                .Callback<string, string, object>((_, __, param) =>
                {
                    var status = Assert.IsType<List<ConnectedEquipmentDto>>(param);
                    Assert.Empty(status);
                });

            // act
            await service.OnEquipmentDisconnected(TestParticipants.Default, EqConnectionId);

            // assert
            _signal.Verify(
                x => x.SendToConnectionAsync(ConnectionId, CoreHubMessages.Response.OnEquipmentUpdated,
                    It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task TestOnEquipmentError()
        {
            var service = await TestEquipmentConnected();
            _signal.Reset();

            // arrange
            var message = TestServiceMessage.Create(EquipmentError.NotFound, TestParticipants.Default, EqConnectionId)
                .Object;

            // act
            await service.EquipmentErrorOccurred(message);

            // assert
            _signal.Verify(
                x => x.SendToConnectionAsync(ConnectionId, CoreHubMessages.Response.OnError, It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task TestSendCommandToNonExistingEquipment()
        {
            var service = await TestEquipmentConnected();
            _signal.Reset();

            // arrange
            var message = TestServiceMessage.Create(
                new EquipmentCommand
                {
                    Action = EquipmentCommandType.Enable,
                    DeviceId = "Dev",
                    EquipmentId = Guid.Parse("a0c54226-13f3-46d6-a48d-a6d457d707c1"),
                    Source = ProducerSource.Mic,
                }, TestParticipants.Default, ConnectionId);

            // act
            var result = await service.SendEquipmentCommand(message.Object);

            // assert
            AssertFailed(result);
        }

        [Fact]
        public async Task TestSendCommand()
        {
            var service = await TestEquipmentConnected();

            // arrange
            var registerMessage = TestServiceMessage.Create(
                new RegisterEquipmentRequestDto
                {
                    Name = "Unit test",
                    Devices = new List<EquipmentDeviceInfo>
                    {
                        new EquipmentDeviceInfo
                        {
                            DeviceId = "test", Label = "Webcam", Source = ProducerSource.Webcam,
                        },
                    },
                }, TestParticipants.Default, EqConnectionId).Object;

            List<ConnectedEquipmentDto>? status = null;

            _signal.Setup(x =>
                    x.SendToConnectionAsync(ConnectionId, CoreHubMessages.Response.OnEquipmentUpdated,
                        It.IsAny<object>()))
                .Callback<string, string, object>((_, __, obj) =>
                    status = Assert.IsType<List<ConnectedEquipmentDto>>(obj));

            // act
            await service.RegisterEquipment(registerMessage);

            Assert.NotNull(status);

            var equipmentId = Assert.Single(status).EquipmentId;

            await service.SendEquipmentCommand(TestServiceMessage
                .Create(
                    new EquipmentCommand
                    {
                        EquipmentId = equipmentId,
                        DeviceId = "test",
                        Action = EquipmentCommandType.Enable,
                        Source = ProducerSource.Webcam,
                    }, TestParticipants.Default, ConnectionId).Object);

            // assert
            _signal.Verify(
                x => x.SendToConnectionAsync(EqConnectionId, CoreHubMessages.Response.OnEquipmentCommand,
                    It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task TestUpdateEquipmentStatus()
        {
            var service = await TestEquipmentConnected();
            _signal.Reset();

            // arrange
            var message = TestServiceMessage.Create(
                new Dictionary<string, UseMediaStateInfo> {{"webcam", new UseMediaStateInfo {Connected = true}}},
                TestParticipants.Default, EqConnectionId);

            // act
            await service.EquipmentUpdateStatus(message.Object);

            // assert
            _signal.Verify(
                x => x.SendToConnectionAsync(ConnectionId, CoreHubMessages.Response.OnEquipmentUpdated,
                    It.IsAny<object>()), Times.Once);
        }
    }
}
