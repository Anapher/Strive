using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Options;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Extensions;
using PaderConference.Core.IntegrationTests.Services.Base;
using PaderConference.Core.Services;
using PaderConference.Core.Services.ConferenceControl.Notifications;
using PaderConference.Core.Services.Permissions;
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

        private readonly SynchronizedObjectId _participantSyncObj =
            SynchronizedParticipantPermissionsProvider.GetObjIdOfParticipant(ParticipantId);

        private static readonly PermissionDescriptor<bool> _testPermission = new("test");

        private static readonly Participant TestParticipant = new(ConferenceId, ParticipantId);

        private class TestPermissionValidator : DefinedPermissionValidator
        {
            public override bool TryGetDescriptor(string permissionKey, out PermissionDescriptor? permissionDescriptor)
            {
                if (base.TryGetDescriptor(permissionKey, out permissionDescriptor)) return true;
                if (permissionKey == _testPermission.Key)
                {
                    permissionDescriptor = _testPermission;
                    return true;
                }

                return false;
            }
        }

        public PermissionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            base.ConfigureContainer(builder);

            AddConferenceRepo(builder, new Conference(ConferenceId));
            builder.RegisterInstance(new OptionsWrapper<DefaultPermissionOptions>(new DefaultPermissionOptions()))
                .AsImplementedInterfaces();

            builder.RegisterInstance(new TestPermissionValidator()).AsImplementedInterfaces();
        }

        protected override IEnumerable<Type> FetchServiceTypes()
        {
            return FetchTypesOfNamespace(typeof(PermissionsError)).Concat(FetchTypesForSynchronizedObjects());
        }

        [Fact]
        public async Task ParticipantInitialized_UpdateSynchronizedObject()
        {
            // arrange
            await SetParticipantJoined(TestParticipant);

            // act
            await Mediator.Publish(new ParticipantInitializedNotification(TestParticipant));
            await Mediator.Publish(new ParticipantJoinedNotification(TestParticipant));

            // assert
            var syncObj =
                SynchronizedObjectListener.GetSynchronizedObject<SynchronizedParticipantPermissions>(TestParticipant,
                    _participantSyncObj);

            Assert.NotNull(syncObj);
        }

        [Fact]
        public async Task SetTemporaryPermission_UpdateSyncObj()
        {
            // arrange
            await SetParticipantJoined(TestParticipant);

            await Mediator.Publish(new ParticipantInitializedNotification(TestParticipant));
            await Mediator.Publish(new ParticipantJoinedNotification(TestParticipant));
            var permission = _testPermission.Configure(true);

            // act
            var result = await Mediator.Send(new SetTemporaryPermissionRequest(TestParticipant, permission.Key,
                permission.Value));

            // assert
            Assert.True(result.Success);
            var syncObj =
                SynchronizedObjectListener.GetSynchronizedObject<SynchronizedParticipantPermissions>(TestParticipant,
                    _participantSyncObj);

            Assert.NotNull(syncObj);
            Assert.Contains(permission.Key, syncObj.Permissions.Keys);
        }

        [Fact]
        public async Task SetTemporaryPermission_UpdateParticipantPermissions()
        {
            // arrange
            await SetParticipantJoined(TestParticipant);

            await Mediator.Publish(new ParticipantInitializedNotification(TestParticipant));
            await Mediator.Publish(new ParticipantJoinedNotification(TestParticipant));
            var permission = _testPermission.Configure(true);

            // act
            var result = await Mediator.Send(new SetTemporaryPermissionRequest(TestParticipant, permission.Key,
                permission.Value));

            // assert
            Assert.True(result.Success);

            var participantPermissions = Container.Resolve<IParticipantPermissions>();
            var stack = await participantPermissions.FetchForParticipant(new Participant(ConferenceId, ParticipantId));
            var actualValue = await stack.GetPermissionValue(_testPermission);
            Assert.True(actualValue);
        }
    }
}
