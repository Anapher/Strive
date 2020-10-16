using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using PaderConference.Infrastructure;
using PaderConference.Infrastructure.Hubs.Dto;
using PaderConference.IntegrationTests._Helpers;
using PaderConference.Models.Request;
using PaderConference.Models.Response;
using Xunit;

namespace PaderConference.IntegrationTests.Controllers
{
    public class ChatTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public ChatTest(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Run()
        {
            var response = await _client.PostAsync("/api/v1/auth/login",
                new JsonContent(new LoginRequestDto {UserName = "Vincent", Password = "123"}));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.DeserializeJsonObject<LoginResponseDto>();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);

            response = await _client.PostAsync("/api/v1/conference",
                new JsonContent(new CreateConferenceRequestDto()));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var conferenceInfo = await response.Content.DeserializeJsonObject<StartConferenceResponseDto>();
            Assert.NotEmpty(conferenceInfo.ConferenceId);

            var connection = new HubConnectionBuilder()
                .WithUrl(
                    $"http://localhost/signalr?access_token={result.AccessToken}&conferenceId={conferenceInfo.ConferenceId}",
                    o => o.HttpMessageHandlerFactory = _ => _factory.Server.CreateHandler())
                .Build();

            List<ChatMessageDto> receivedMessages = null;

            connection.On<ChatMessageDto>(CoreHubMessages.Response.ChatMessage,
                message => receivedMessages.Add(message));
            connection.On<List<ChatMessageDto>>(CoreHubMessages.Response.Chat,
                messages => receivedMessages = messages);

            await connection.StartAsync();

            await connection.InvokeAsync(CoreHubMessages.Request.RequestChat);
            await Task.Delay(100);

            Assert.NotNull(receivedMessages);
            Assert.Empty(receivedMessages);

            await connection.InvokeAsync(CoreHubMessages.Request.SendChatMessage,
                new SendChatMessageDto {Message = "Hello World"});

            var message = Assert.Single(receivedMessages);
            Assert.Equal("Hello World", message.Message);

            receivedMessages.Clear();

            await connection.InvokeAsync(CoreHubMessages.Request.RequestChat);
            await Task.Delay(100);

            message = Assert.Single(receivedMessages);
            Assert.Equal("Hello World", message.Message);
        }
    }
}