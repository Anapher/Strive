using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Services;
using PaderConference.Core.Services.ConferenceControl.Gateways;
using PaderConference.Core.Services.ConferenceControl.Notifications;
using PaderConference.Core.Services.ConferenceControl.Requests;
using PaderConference.Core.Services.ConferenceControl.UseCases;
using PaderConference.Core.Tests._TestHelpers;
using PaderConference.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.Core.Tests.Services.ConferenceControl.UseCases
{
    public class OpenConferenceHandlerTests
    {
        private readonly ILogger<OpenConferenceHandler> _logger;
        private readonly Mock<IMediator> _mediator;
        private readonly Mock<IOpenConferenceRepository> _openConferenceRepository;

        public OpenConferenceHandlerTests(ITestOutputHelper outputHelper)
        {
            _logger = outputHelper.CreateLogger<OpenConferenceHandler>();

            _openConferenceRepository = new Mock<IOpenConferenceRepository>();
            _mediator = new Mock<IMediator>();
        }

        private OpenConferenceHandler CreateHandler()
        {
            return new(_openConferenceRepository.Object, _mediator.Object, _logger);
        }

        private void InitializeConferenceInRepository(string conferenceId)
        {
            ConferenceManagementHelper.SetConferenceExists(_mediator, new Conference(conferenceId));
        }

        private void SetConferenceIsOpen(string conferenceId)
        {
            _openConferenceRepository.Setup(x => x.Create(conferenceId)).ReturnsAsync(false);
        }

        private void SetConferenceIsClosed(string conferenceId)
        {
            _openConferenceRepository.Setup(x => x.Create(conferenceId)).ReturnsAsync(true);
        }

        [Fact]
        public async Task Handle_ConferenceDoesNotExist_ThrowConferenceNotFoundException()
        {
            const string conferenceId = "test";

            // arrange
            SetConferenceIsClosed(conferenceId);
            var handler = CreateHandler();

            ConferenceManagementHelper.SetConferenceDoesNotExist(_mediator, conferenceId);

            // act & assert
            await Assert.ThrowsAsync<ConferenceNotFoundException>(async () =>
            {
                await handler.Handle(new OpenConferenceRequest(conferenceId), CancellationToken.None);
            });

            ConferenceManagementHelper.VerifyAnyFindConferenceById(_mediator);
            _mediator.VerifyNoOtherCalls();
            _openConferenceRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_ConferenceAlreadyOpen_NoExceptionAndDontPublishNotification()
        {
            const string conferenceId = "test";

            // arrange
            SetConferenceIsOpen(conferenceId);
            InitializeConferenceInRepository(conferenceId);

            var handler = CreateHandler();

            // act
            await handler.Handle(new OpenConferenceRequest(conferenceId), CancellationToken.None);

            // assert
            ConferenceManagementHelper.VerifyAnyFindConferenceById(_mediator);
            _mediator.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_ConferenceClosed_OpenConferenceAndPublishNotification()
        {
            const string conferenceId = "test";

            // arrange
            SetConferenceIsClosed(conferenceId);
            InitializeConferenceInRepository(conferenceId);

            var capturedNotification = _mediator.CaptureNotification<ConferenceOpenedNotification>();

            var handler = CreateHandler();

            // act
            await handler.Handle(new OpenConferenceRequest(conferenceId), CancellationToken.None);

            // assert
            var notification = capturedNotification.GetNotification();
            Assert.Equal(conferenceId, notification.ConferenceId);
        }
    }
}
