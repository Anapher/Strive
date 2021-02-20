using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using PaderConference.Core.Services;
using PaderConference.Core.Services.Permissions;
using PaderConference.Core.Services.Permissions.Gateways;
using PaderConference.Core.Services.Permissions.Requests;
using PaderConference.Core.Services.Permissions.UseCases;
using PaderConference.Core.Tests._TestHelpers;
using PaderConference.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.Core.Tests.Services.Permissions.UseCases
{
    public class SetTemporaryPermissionHandlerTests
    {
        private readonly Mock<IMediator> _mediator = new();
        private readonly Mock<ITemporaryPermissionRepository> _temporaryPermissions = new();
        private readonly JoinedParticipantsRepositoryMock _joinedParticipants = new();
        private readonly Mock<IPermissionValidator> _permissionValidator = new();
        private readonly ILogger<SetTemporaryPermissionHandler> _logger;

        private const string ParticipantId = "123";
        private const string ConferenceId = "45";

        private const string PermissionKey = "test";
        private readonly JValue _permissionValue = (JValue) JToken.FromObject(false);

        private readonly PermissionDescriptor _testPermissionDescriptor =
            new(PermissionKey, PermissionValueType.Boolean);

        private static readonly Participant Participant = new(ConferenceId, ParticipantId);

        public SetTemporaryPermissionHandlerTests(ITestOutputHelper testOutputHelper)
        {
            _logger = testOutputHelper.CreateLogger<SetTemporaryPermissionHandler>();
        }

        private SetTemporaryPermissionHandler Create()
        {
            return new(_mediator.Object, _temporaryPermissions.Object, _joinedParticipants.Object,
                _permissionValidator.Object, _logger);
        }

        private void SetValidPermission(PermissionDescriptor descriptor)
        {
            _permissionValidator.Setup(x => x.TryGetDescriptor(descriptor.Key, out descriptor!)).Returns(true);
        }

        [Fact]
        public async Task
            Handle_ValidValueAndParticipantIsJoined_SetParticipantPermissionsInRepositoryAndPublishUpdateRequest()
        {
            // assert
            var capturedRequest = _mediator.CaptureRequest<UpdateParticipantsPermissionsRequest, Unit>();

            SetValidPermission(_testPermissionDescriptor);
            _joinedParticipants.JoinParticipant(Participant);

            var handler = Create();
            var request = new SetTemporaryPermissionRequest(Participant, PermissionKey, _permissionValue);

            // act
            var result = await handler.Handle(request, CancellationToken.None);

            // assert
            Assert.True(result.Success);
            _temporaryPermissions.Verify(x => x.SetTemporaryPermission(Participant, PermissionKey, _permissionValue),
                Times.Once);

            capturedRequest.AssertReceived();
            var updateRequest = capturedRequest.GetRequest();
            Assert.Equal(Participant, Assert.Single(updateRequest.Participants));
        }

        [Fact]
        public async Task Handle_PermissionKeyDoesNotExist_ReturnFalseAndDontSetNewPermission()
        {
            // assert
            _joinedParticipants.JoinParticipant(Participant);

            var handler = Create();
            var request = new SetTemporaryPermissionRequest(Participant, PermissionKey, _permissionValue);

            // act
            var result = await handler.Handle(request, CancellationToken.None);

            // assert
            Assert.False(result.Success);
            Assert.Equal(PermissionsError.PermissionKeyNotFound(string.Empty).Code, result.Error?.Code);

            _temporaryPermissions.VerifyNoOtherCalls();
            _mediator.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_InvalidPermissionValue_ReturnFalseAndDontSetNewPermission()
        {
            // assert
            _joinedParticipants.JoinParticipant(Participant);
            SetValidPermission(_testPermissionDescriptor);

            var handler = Create();

            var invalidValue = (JValue) JToken.FromObject("test");
            var request = new SetTemporaryPermissionRequest(Participant, PermissionKey, invalidValue);

            // act
            var result = await handler.Handle(request, CancellationToken.None);

            // assert
            Assert.False(result.Success);
            Assert.Equal(PermissionsError.InvalidPermissionValueType.Code, result.Error?.Code);

            _temporaryPermissions.VerifyNoOtherCalls();
            _mediator.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_ParticipantNotJoined_ReturnFalseAndRemovedPermissions()
        {
            // assert
            SetValidPermission(_testPermissionDescriptor);

            var handler = Create();
            var request = new SetTemporaryPermissionRequest(Participant, PermissionKey, _permissionValue);

            // act
            var result = await handler.Handle(request, CancellationToken.None);

            // assert
            Assert.False(result.Success);

            _temporaryPermissions.Verify(x => x.RemoveAllTemporaryPermissions(Participant), Times.Once);
            _mediator.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_NullValue_RemovePermission()
        {
            // assert
            var capturedRequest = _mediator.CaptureRequest<UpdateParticipantsPermissionsRequest, Unit>();

            SetValidPermission(_testPermissionDescriptor);
            _joinedParticipants.JoinParticipant(Participant);

            var handler = Create();
            var request = new SetTemporaryPermissionRequest(Participant, PermissionKey, null);

            // act
            var result = await handler.Handle(request, CancellationToken.None);

            // assert
            Assert.True(result.Success);

            _temporaryPermissions.Verify(x => x.RemoveTemporaryPermission(Participant, PermissionKey), Times.Once);
            capturedRequest.AssertReceived();
        }
    }
}
