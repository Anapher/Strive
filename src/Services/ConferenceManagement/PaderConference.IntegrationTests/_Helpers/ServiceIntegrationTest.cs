using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Dto;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Services;
using PaderConference.Hubs;
using PaderConference.Models.Request;
using PaderConference.Models.Response;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.IntegrationTests._Helpers
{
    public abstract class ServiceIntegrationTest : IntegrationTestBase
    {
        protected readonly UserAccount Moderator;

        protected ServiceIntegrationTest(ITestOutputHelper testOutputHelper, MongoDbFixture mongoDb) : base(
            testOutputHelper, mongoDb)
        {
            Moderator = CreateUser(true, "Vincent");
        }

        protected UserAccount CreateUser(bool isModerator = false, string? name = null)
        {
            return Factory.CreateUser(name ?? "Olaf", isModerator).SetupHttpClient(Client);
        }

        protected Task<ConferenceCreatedResponseDto> CreateConference(params UserAccount[] moderators)
        {
            var creationDto = new CreateConferenceRequestDto
            {
                Configuration = new ConferenceConfiguration
                {
                    Moderators = new[] {"0"}.Concat(moderators.Select(x => x.Sub)).ToImmutableList(),
                },
                Permissions = new Dictionary<PermissionType, Dictionary<string, JValue>>(),
            };

            return CreateConference(creationDto);
        }

        protected async Task<ConferenceCreatedResponseDto> CreateConference(CreateConferenceRequestDto creationDto)
        {
            var response = await Client.PostAsJsonAsync("/v1/conference", creationDto);
            response.EnsureSuccessStatusCode();

            var createdConference = await response.Content.ReadFromJsonAsync<ConferenceCreatedResponseDto>();
            Assert.NotNull(createdConference);

            Logger.Information("Created conference {conferenceId}", createdConference?.ConferenceId);

            return createdConference!;
        }

        protected async Task<(UserConnection, ConferenceCreatedResponseDto)> ConnectToOpenedConference()
        {
            var conference = await CreateConference(Moderator);
            var connection = await ConnectUserToConference(Moderator, conference);
            var result = await OpenConference(connection);
            AssertSuccess(result);

            return (connection, conference);
        }

        protected async Task<UserConnection> ConnectUserToConference(UserAccount user,
            ConferenceCreatedResponseDto conference)
        {
            var conferenceId = conference.ConferenceId;
            var connection = CreateHubConnection(user, conferenceId);

            var syncObjListener = SynchronizedObjectListener.Initialize(connection, Logger);

            Logger.Information("Establish connection to SignalR for conference {conferenceId}", conferenceId);
            await connection.StartAsync();
            Logger.Information("Connection to SignalR established.");

            var result = new UserConnection(connection, conferenceId, user, syncObjListener);
            await EnsureClientJoinCompleted(result);

            return result;
        }

        protected async Task<SuccessOrError<Unit>> OpenConference(UserConnection userConnection)
        {
            return await userConnection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.OpenConference));
        }

        protected HubConnection CreateHubConnection(UserAccount user, string conferenceId)
        {
            return new HubConnectionBuilder()
                .WithUrl($"http://localhost/signalr?access_token={user.Token}&conferenceId={conferenceId}",
                    o => o.HttpMessageHandlerFactory = _ => Factory.Server.CreateHandler()).AddNewtonsoftJsonProtocol()
                .Build();
        }

        protected async Task EnsureClientJoinCompleted(UserConnection connection)
        {
            await connection.Hub.InvokeAsync(nameof(CoreHub.FetchPermissions), null);
        }

        protected void AssertErrorCode(ServiceErrorCode code, Error error)
        {
            Assert.Equal(code.ToString(), error.Code);
        }

        protected void AssertSuccess<T>(SuccessOrError<T> successOrError)
        {
            Assert.True(successOrError.Success, $"Failed with error: {successOrError.Error?.Code}");
        }

        protected void AssertFailed<T>(SuccessOrError<T> successOrError)
        {
            Assert.False(successOrError.Success);
        }
    }

    public record UserConnection(HubConnection Hub, string ConferenceId, UserAccount User,
        SynchronizedObjectListener SyncObjects);
}
