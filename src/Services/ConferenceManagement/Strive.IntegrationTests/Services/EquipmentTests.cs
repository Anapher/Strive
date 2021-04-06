using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR.Client;
using Strive.Core.Dto;
using Strive.Core.Interfaces;
using Strive.Core.Services;
using Strive.Core.Services.Equipment;
using Strive.Core.Services.Media;
using Strive.Core.Services.Media.Dtos;
using Strive.Hubs.Core;
using Strive.Hubs.Core.Dtos;
using Strive.Hubs.Core.Responses;
using Strive.Hubs.Equipment;
using Strive.Hubs.Equipment.Dtos;
using Strive.Hubs.Equipment.Responses;
using Strive.IntegrationTests._Helpers;
using Strive.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Strive.IntegrationTests.Services
{
    [Collection(IntegrationTestCollection.Definition)]
    public class EquipmentTests : ServiceIntegrationTest
    {
        private const string EquipmentName = "Integration Test Equipment";

        public EquipmentTests(ITestOutputHelper testOutputHelper, MongoDbFixture mongoDb) : base(testOutputHelper,
            mongoDb)
        {
        }

        private static string BuildEquipmentSignalrUrl(Participant participant, string token)
        {
            return
                $"http://localhost/equipment-signalr?token={token}&conferenceId={participant.ConferenceId}&participantId={participant.Id}";
        }

        private async Task<(UserConnection, HubConnection)> ConnectEquipment(IReadOnlyList<EquipmentDevice> devices)
        {
            var (connection, conference) = await ConnectToOpenedConference();
            var participant = new Participant(conference.ConferenceId, Moderator.Sub);

            var result = await connection.Hub.InvokeAsync<SuccessOrError<string>>(nameof(CoreHub.GetEquipmentToken));
            AssertSuccess(result);

            var equipmentHubUrl = BuildEquipmentSignalrUrl(participant, result.Response!);
            var equipmentHub = CreateHubConnection(equipmentHubUrl);
            await equipmentHub.StartAsync();

            var initResult = await equipmentHub.InvokeAsync<SuccessOrError<Unit>>(nameof(EquipmentHub.Initialize),
                new InitializeEquipmentDto(EquipmentName, devices));
            AssertSuccess(initResult);

            return (connection, equipmentHub);
        }

        [Fact]
        public async Task InitializeEquipment_WithValidToken_UpdateSynchronizedObject()
        {
            // arrange
            var testMic = new EquipmentDevice("mic1", "Microphone", DeviceType.Mic);
            var testDevices = new List<EquipmentDevice> {testMic};

            var (user, _) = await ConnectEquipment(testDevices);

            // assert
            await user.SyncObjects.AssertSyncObject<SynchronizedEquipment>(
                SynchronizedEquipment.SyncObjId(user.User.Sub), syncEquipment =>
                {
                    var connection = Assert.Single(syncEquipment.Connections);
                    Assert.Equal(EquipmentName, connection.Value.Name);
                    Assert.Empty(connection.Value.Status);
                    var actualDevice = Assert.Single(connection.Value.Devices);

                    Assert.Equal(testMic.DeviceId, actualDevice.Key);
                    Assert.Equal(testMic, actualDevice.Value);
                });
        }

        [Fact]
        public async Task InitializeEquipment_WithInvalidToken_AbortConnection()
        {
            var (_, conference) = await ConnectToOpenedConference();
            var participant = new Participant(conference.ConferenceId, Moderator.Sub);

            var equipmentHubUrl = BuildEquipmentSignalrUrl(participant, "fake token");
            var equipmentHub = CreateHubConnection(equipmentHubUrl);
            await equipmentHub.StartAsync();

            await AssertHelper.WaitForAssert(() => Assert.Equal(HubConnectionState.Disconnected, equipmentHub.State));
        }

        [Fact]
        public async Task UpdateStatus_ConnectedEquipment_UpdateSynchronizedObject()
        {
            // arrange
            var testMic = new EquipmentDevice("mic1", "Microphone", DeviceType.Mic);
            var testDevices = new List<EquipmentDevice> {testMic};

            var (user, equipmentHub) = await ConnectEquipment(testDevices);

            var update =
                new Dictionary<string, UseMediaStateInfo> {{"mic1", new UseMediaStateInfo(true, true, false, null)}};

            // act
            await equipmentHub.InvokeAsync(nameof(EquipmentHub.UpdateStatus), update);

            // assert
            await user.SyncObjects.AssertSyncObject<SynchronizedEquipment>(
                SynchronizedEquipment.SyncObjId(user.User.Sub), syncObj =>
                {
                    var connection = Assert.Single(syncObj.Connections).Value;
                    Assert.Equal(update, connection.Status);
                });
        }

        [Fact]
        public async Task Disconnect_EquipmentConnected_RemoveFromSynchronizedObject()
        {
            // arrange
            var testDevices = Array.Empty<EquipmentDevice>();

            var (user, equipmentHub) = await ConnectEquipment(testDevices);
            await user.SyncObjects.AssertSyncObject<SynchronizedEquipment>(
                SynchronizedEquipment.SyncObjId(user.User.Sub),
                syncEquipment => { Assert.Single(syncEquipment.Connections); });

            // act
            await equipmentHub.DisposeAsync();

            // assert
            await user.SyncObjects.AssertSyncObject<SynchronizedEquipment>(
                SynchronizedEquipment.SyncObjId(user.User.Sub),
                syncEquipment => { Assert.Empty(syncEquipment.Connections); });
        }

        [Fact]
        public async Task ParticipantDisconnect_EquipmentConnected_SendDisconnectMessage()
        {
            // arrange
            var testDevices = Array.Empty<EquipmentDevice>();
            var (user, equipmentHub) = await ConnectEquipment(testDevices);

            var requestDisconnect = false;
            equipmentHub.On(EquipmentHubMessages.OnRequestDisconnect, () => requestDisconnect = true);

            // act
            await user.Hub.DisposeAsync();

            // assert
            await AssertHelper.WaitForAssert(() => Assert.True(requestDisconnect));
        }

        [Fact]
        public async Task ParticipantDisconnect_EquipmentConnected_ErrorOnHubInvoke()
        {
            // arrange
            var testDevices = Array.Empty<EquipmentDevice>();
            var (user, equipmentHub) = await ConnectEquipment(testDevices);

            await user.Hub.DisposeAsync();

            await AssertHelper.WaitForAssertAsync(async () =>
            {
                // act
                var data = new Dictionary<string, UseMediaStateInfo>();
                var result =
                    await equipmentHub.InvokeAsync<SuccessOrError<Unit>>(nameof(EquipmentHub.UpdateStatus), data);

                // assert
                AssertFailed(result);
            });
        }

        [Fact]
        public async Task ParticipantDisconnected_ConnectEquipment_FailConnection()
        {
            var (connection, conference) = await ConnectToOpenedConference();
            var participant = new Participant(conference.ConferenceId, Moderator.Sub);

            var result = await connection.Hub.InvokeAsync<SuccessOrError<string>>(nameof(CoreHub.GetEquipmentToken));
            AssertSuccess(result);

            await connection.Hub.DisposeAsync();

            var equipmentHubUrl = BuildEquipmentSignalrUrl(participant, result.Response!);
            var equipmentHub = CreateHubConnection(equipmentHubUrl);
            await equipmentHub.StartAsync();

            try
            {
                var initResult = await equipmentHub.InvokeAsync<SuccessOrError<Unit>>(nameof(EquipmentHub.Initialize),
                    new InitializeEquipmentDto(EquipmentName, Array.Empty<EquipmentDevice>()));
                AssertFailed(initResult);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        [Fact]
        public async Task SendEquipmentCommand_EquipmentConnected_ReceiveCommandAsEquipment()
        {
            // arrange
            var testDevices = Array.Empty<EquipmentDevice>();
            var (user, equipmentHub) = await ConnectEquipment(testDevices);

            EquipmentCommandDto? commandDto = null;
            equipmentHub.On(EquipmentHubMessages.OnEquipmentCommand, (EquipmentCommandDto dto) => commandDto = dto);

            // act
            var sendDto = new SendEquipmentCommandDto(equipmentHub.ConnectionId, ProducerSource.Mic, null,
                EquipmentCommandType.Enable);

            var result =
                await user.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SendEquipmentCommand), sendDto);

            // assert
            AssertSuccess(result);
            await AssertHelper.WaitForAssert(() =>
            {
                Assert.NotNull(commandDto);
                Assert.Equal(sendDto.Action, commandDto!.Action);
                Assert.Equal(sendDto.DeviceId, commandDto.DeviceId);
                Assert.Equal(sendDto.Source, commandDto.Source);
            });
        }

        [Fact]
        public async Task OnEquipmentError_EquipmentConnected_ReceiveErrorAsParticipant()
        {
            // arrange
            var testDevices = Array.Empty<EquipmentDevice>();
            var (user, equipmentHub) = await ConnectEquipment(testDevices);

            EquipmentErrorDto? equipmentError = null;
            user.Hub.On(CoreHubMessages.OnEquipmentError, (EquipmentErrorDto err) => equipmentError = err);

            // act
            var error = new Error("test type", "test message", "test code");
            var result =
                await equipmentHub.InvokeAsync<SuccessOrError<Unit>>(nameof(EquipmentHub.ErrorOccurred), error);

            // assert
            AssertSuccess(result);
            await AssertHelper.WaitForAssert(() =>
            {
                Assert.NotNull(equipmentError);

                Assert.Equal(equipmentHub.ConnectionId, equipmentError!.ConnectionId);
                Assert.Equal(error.Message, equipmentError.Error.Message);
                Assert.Equal(error.Code, equipmentError.Error.Code);
                Assert.Equal(error.Type, equipmentError.Error.Type);
            });
        }

        [Fact]
        public async Task FetchSfuConnectionInfo_EquipmentConnected_GetTokenAndUrl()
        {
            // arrange
            var testDevices = Array.Empty<EquipmentDevice>();
            var (user, equipmentHub) = await ConnectEquipment(testDevices);

            // act
            var connectionInfo =
                await equipmentHub.InvokeAsync<SuccessOrError<SfuConnectionInfo>>(
                    nameof(EquipmentHub.FetchSfuConnectionInfo));

            // assert
            AssertSuccess(connectionInfo);
            Assert.NotNull(connectionInfo.Response?.Url);
        }
    }
}
