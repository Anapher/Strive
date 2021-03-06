using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Dto;
using PaderConference.Core.Extensions;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Services;
using PaderConference.Hubs;
using PaderConference.Models.Request;
using Serilog;
using Serilog.Core;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.IntegrationTests._Helpers
{
    public abstract class ServiceIntegrationTest : IClassFixture<CustomWebApplicationFactory>
    {
        protected readonly CustomWebApplicationFactory Factory;
        protected readonly Logger Logger;
        protected readonly HttpClient Client;

        protected ServiceIntegrationTest(CustomWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
        {
            Factory = factory;
            Logger = testOutputHelper.CreateTestLogger();
            Client = factory.CreateClient();
        }

        protected async Task<ConnectedUser> InitializeConferenceAndConnect(bool isModerator = false)
        {
            var user = Factory.CreateUser("Vincent", true).SetupHttpClient(Client);
            Logger.Information("Created user {sub}, user is moderator", user.Sub);

            var creationDto = new CreateConferenceRequestDto
            {
                Configuration = new ConferenceConfiguration
                {
                    Moderators = new[] {"0"}.Concat(isModerator ? user.Sub.Yield() : Array.Empty<string>())
                        .ToImmutableList(),
                },
                Permissions = new Dictionary<PermissionType, Dictionary<string, JValue>>(),
            };

            //var response = await Client.PostAsJsonAsync("/v1/conference", creationDto);
            //response.EnsureSuccessStatusCode();

            //var createdConference = await response.Content.ReadFromJsonAsync<ConferenceCreatedResponseDto>();
            //Assert.NotNull(createdConference);

            //var conferenceId = createdConference!.ConferenceId;
            //Logger.Information("Created conference {conferenceId}", conferenceId);

            //var connection = CreateHubConnection(user, conferenceId);

            //var syncObjListener = SynchronizedObjectListener.Initialize(connection, Logger);

            //Logger.Information("Establish connection to SignalR for conference {conferenceId}", conferenceId);
            //await connection.StartAsync();
            //Logger.Information("Connection to SignalR established.");

            return null;
        }

        protected async Task<SuccessOrError<Unit>> OpenConference(ConnectedUser connectedUser)
        {
            return await connectedUser.Connection.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.OpenConference));
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

    public record ConnectedUser(HubConnection Connection, string ConferenceId, UserAccount User,
        SynchronizedObjectListener SyncObjects);
}
