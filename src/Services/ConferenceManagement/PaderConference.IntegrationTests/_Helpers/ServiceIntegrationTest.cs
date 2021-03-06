using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
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
using Serilog;
using Serilog.Core;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.IntegrationTests._Helpers
{
    public abstract class ServiceIntegrationTest
    {
        protected readonly CustomWebApplicationFactory Factory;
        protected readonly Logger Logger;
        protected readonly HttpClient Client;
        protected readonly UserAccount Moderator;

        protected ServiceIntegrationTest(ITestOutputHelper testOutputHelper, MongoDbFixture mongoDb)
        {
            Factory = new CustomWebApplicationFactory(mongoDb, testOutputHelper);
            Logger = testOutputHelper.CreateTestLogger();
            Client = Factory.CreateClient();

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
            Assert.True(result.Success);

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

            return new UserConnection(connection, conferenceId, user, syncObjListener);
        }

        protected async Task<SuccessOrError<Unit>> OpenConference(UserConnection userConnection)
        {
            return await userConnection.Connection.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.OpenConference));
        }

        protected HubConnection CreateHubConnection(UserAccount user, string conferenceId)
        {
            return new HubConnectionBuilder()
                .WithUrl($"http://localhost/signalr?access_token={user.Token}&conferenceId={conferenceId}",
                    o => o.HttpMessageHandlerFactory = _ => Factory.Server.CreateHandler()).AddNewtonsoftJsonProtocol()
                .Build();
        }

        protected void AssertErrorCode(ServiceErrorCode code, Error error)
        {
            Assert.Equal(code.ToString(), error.Code);
        }
    }

    public record UserConnection(HubConnection Connection, string ConferenceId, UserAccount User,
        SynchronizedObjectListener SyncObjects);
}
