using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Moq;
using Newtonsoft.Json.Linq;
using PaderConference.Core.Extensions;
using PaderConference.Core.Services;
using PaderConference.Core.Services.Permissions;
using PaderConference.Hubs.Services;
using PaderConference.Hubs.Services.Middlewares;
using Xunit;

namespace PaderConference.Tests.Hubs.Services.Middlewares
{
    public class ServiceInvokerPermissionMiddlewareTests : MiddlewareTestBase
    {
        protected override IServiceRequestBuilder<string> Execute(IServiceRequestBuilder<string> builder)
        {
            return builder.RequirePermissions(new PermissionDescriptor<bool>("test"));
        }

        [Fact]
        public async Task CheckPermissions_NoPermissions_Succeed()
        {
            // arrange
            var context = CreateContext();

            // act
            var result =
                await ServiceInvokerPermissionMiddleware.CheckPermissions(context,
                    Array.Empty<PermissionDescriptor<bool>>());

            // assert
            Assert.True(result.Success);
        }

        [Fact]
        public async Task CheckPermissions_HasOnePermission_Succeed()
        {
            // arrange
            var permission = new PermissionDescriptor<bool>("Test");
            var participantPermissions = CreateParticipantPermissions(permission.Configure(true));

            var context = CreateContext(builder =>
                builder.RegisterInstance(participantPermissions).AsImplementedInterfaces());

            // act
            var result = await ServiceInvokerPermissionMiddleware.CheckPermissions(context, permission);

            // assert
            Assert.True(result.Success);
        }

        [Fact]
        public async Task CheckPermissions_DoesntHavePermission_Fail()
        {
            // arrange
            var permission = new PermissionDescriptor<bool>("Test");
            var participantPermissions = CreateParticipantPermissions();

            var context = CreateContext(builder =>
                builder.RegisterInstance(participantPermissions).AsImplementedInterfaces());

            // act
            var result = await ServiceInvokerPermissionMiddleware.CheckPermissions(context, permission);

            // assert
            Assert.False(result.Success);
            Assert.Equal(result.Error.Code, CommonError.PermissionDenied(permission).Code);
        }

        [Fact]
        public async Task CheckPermissions_RequireMultiplePermissionsAndDoesntHaveAll_Fail()
        {
            // arrange
            var permission = new PermissionDescriptor<bool>("Test");
            var permission2 = new PermissionDescriptor<bool>("Test2");
            var participantPermissions = CreateParticipantPermissions(permission.Configure(true));

            var context = CreateContext(builder =>
                builder.RegisterInstance(participantPermissions).AsImplementedInterfaces());

            // act
            var result = await ServiceInvokerPermissionMiddleware.CheckPermissions(context, permission, permission2);

            // assert
            Assert.False(result.Success);
        }

        [Fact]
        public async Task CheckPermissions_RequireMultiplePermissionsAndHasAll_Succeed()
        {
            // arrange
            var permission = new PermissionDescriptor<bool>("Test");
            var permission2 = new PermissionDescriptor<bool>("Test2");
            var participantPermissions =
                CreateParticipantPermissions(permission.Configure(true), permission2.Configure(true));

            var context = CreateContext(builder =>
                builder.RegisterInstance(participantPermissions).AsImplementedInterfaces());

            // act
            var result = await ServiceInvokerPermissionMiddleware.CheckPermissions(context, permission, permission2);

            // assert
            Assert.True(result.Success);
        }

        private IParticipantPermissions CreateParticipantPermissions(params KeyValuePair<string, JValue>[] permissions)
        {
            var permissionStack = CreatePermissionStack(permissions);
            var participant = new Participant(ConferenceId, ParticipantId);

            var participantPermissions = new Mock<IParticipantPermissions>();
            participantPermissions.Setup(x => x.FetchForParticipant(participant))
                .ReturnsAsync(permissionStack);

            return participantPermissions.Object;
        }

        private IPermissionStack CreatePermissionStack(params KeyValuePair<string, JValue>[] permissions)
        {
            return new CachedPermissionStack(new[] {new Dictionary<string, JValue>(permissions)});
        }
    }
}