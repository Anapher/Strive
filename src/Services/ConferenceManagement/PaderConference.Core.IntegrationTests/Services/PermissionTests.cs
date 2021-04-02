using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json.Linq;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Extensions;
using PaderConference.Core.IntegrationTests.Services.Base;
using PaderConference.Core.Services;
using PaderConference.Core.Services.ConferenceControl;
using PaderConference.Core.Services.ConferenceControl.Notifications;
using PaderConference.Core.Services.ConferenceControl.Requests;
using PaderConference.Core.Services.Permissions;
using PaderConference.Core.Services.Permissions.Gateways;
using PaderConference.Core.Services.Permissions.Options;
using PaderConference.Core.Services.Permissions.Requests;
using PaderConference.Core.Services.Synchronization;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.Core.IntegrationTests.Services
{
    public class PermissionTests : ServiceIntegrationTest
    {
        private const string ParticipantId = "123";
        private const string ConferenceId = "45";
        private const string ConnectionId = "connId";
        private const string Username = "Vincent";

        private readonly SynchronizedObjectId _participantSyncObj =
            SynchronizedParticipantPermissions.SyncObjId(ParticipantId);

        private static readonly PermissionDescriptor<bool> TestPermission = new("test");

        private static readonly Participant TestParticipant = new(ConferenceId, ParticipantId);
        private readonly Mock<IPermissionLayerProvider> _permissionLayerProvider = new();

        private class TestPermissionValidator : DefinedPermissionValidator
        {
            public override bool TryGetDescriptor(string permissionKey,
                [NotNullWhen(true)] out PermissionDescriptor? permissionDescriptor)
            {
                if (base.TryGetDescriptor(permissionKey, out permissionDescriptor)) return true;
                if (permissionKey == TestPermission.Key)
                {
                    permissionDescriptor = TestPermission;
                    return true;
                }

                return false;
            }
        }

        public PermissionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _permissionLayerProvider.Setup(x => x.FetchPermissionsForParticipant(It.IsAny<Participant>()))
                .ReturnsAsync(Enumerable.Empty<PermissionLayer>());
        }

        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            base.ConfigureContainer(builder);

            AddConferenceRepo(builder, new Conference(ConferenceId));
            SetupConferenceControl(builder);

            builder.RegisterInstance(new OptionsWrapper<DefaultPermissionOptions>(new DefaultPermissionOptions()))
                .AsImplementedInterfaces();

            builder.RegisterInstance(new TestPermissionValidator()).AsImplementedInterfaces();
            builder.RegisterInstance(_permissionLayerProvider.Object).AsImplementedInterfaces();
        }

        protected override IEnumerable<Type> FetchServiceTypes()
        {
            return FetchTypesOfNamespace(typeof(PermissionsError)).Concat(FetchTypesForSynchronizedObjects());
        }

        private JoinConferenceRequest CreateTestParticipantJoinRequest()
        {
            return new(TestParticipant, ConnectionId, new ParticipantMetadata(Username));
        }

        private SynchronizedParticipantPermissions GetSyncObj()
        {
            return SynchronizedObjectListener.GetSynchronizedObject<SynchronizedParticipantPermissions>(TestParticipant,
                _participantSyncObj);
        }

        [Fact]
        public async Task ParticipantInitialized_UpdateSynchronizedObject()
        {
            // act
            await Mediator.Send(CreateTestParticipantJoinRequest());

            // assert
            GetSyncObj();
        }

        [Fact]
        public async Task SetTemporaryPermission_UpdateSyncObj()
        {
            // arrange
            await Mediator.Send(CreateTestParticipantJoinRequest());

            var permission = TestPermission.Configure(true);

            // act
            var result = await Mediator.Send(new SetTemporaryPermissionRequest(TestParticipant, permission.Key,
                permission.Value));

            // assert
            Assert.True(result.Success);

            var syncObj = GetSyncObj();
            Assert.Contains(permission.Key, syncObj.Permissions.Keys);
        }

        [Fact]
        public async Task SetTemporaryPermission_UpdateParticipantPermissions()
        {
            // arrange
            await Mediator.Send(CreateTestParticipantJoinRequest());

            var permission = TestPermission.Configure(true);

            // act
            var result = await Mediator.Send(new SetTemporaryPermissionRequest(TestParticipant, permission.Key,
                permission.Value));

            // assert
            Assert.True(result.Success);

            await AssertTestParticipantHasTestPermissionValue(true);
        }

        [Fact]
        public async Task UpdateParticipantsPermissions_NewPermissionsAvailable()
        {
            // arrange
            await Mediator.Send(CreateTestParticipantJoinRequest());

            await AssertTestParticipantHasTestPermissionValue(false);

            // act
            var permissionLayer = new PermissionLayer(0, "test",
                new Dictionary<string, JValue>(TestPermission.Configure(true).Yield()));

            _permissionLayerProvider.Setup(x => x.FetchPermissionsForParticipant(TestParticipant))
                .ReturnsAsync(permissionLayer.Yield);

            await Mediator.Send(new UpdateParticipantsPermissionsRequest(TestParticipant.Yield()));

            // assert
            await AssertTestParticipantHasTestPermissionValue(true);
        }

        [Fact]
        public async Task ParticipantLeft_ClearCachedPermissions()
        {
            // arrange
            await Mediator.Send(CreateTestParticipantJoinRequest());

            await Mediator.Send(new SetTemporaryPermissionRequest(TestParticipant, TestPermission.Key,
                (JValue) JToken.FromObject(true)));

            // act
            await Mediator.Publish(new ParticipantLeftNotification(TestParticipant, "any"));

            // assert
            var repo = Container.Resolve<IAggregatedPermissionRepository>();
            var cachedPermissions = await repo.GetPermissions(TestParticipant);
            Assert.Empty(cachedPermissions);

            var tempRepo = Container.Resolve<ITemporaryPermissionRepository>();
            var tempPermissions = await tempRepo.FetchTemporaryPermissions(TestParticipant);
            Assert.Empty(tempPermissions);
        }

        private async Task AssertTestParticipantHasTestPermissionValue(bool expected)
        {
            var participantPermissions = Container.Resolve<IParticipantPermissions>();
            var currentPermissions = await participantPermissions.FetchForParticipant(TestParticipant);
            var actualValue = await currentPermissions.GetPermissionValue(TestPermission);

            Assert.Equal(expected, actualValue);
        }
    }
}
