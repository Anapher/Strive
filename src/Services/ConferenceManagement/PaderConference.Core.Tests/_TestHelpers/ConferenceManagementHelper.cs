using System.Threading;
using MediatR;
using Moq;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Services;
using PaderConference.Core.Services.ConferenceManagement.Requests;

namespace PaderConference.Core.Tests._TestHelpers
{
    public static class ConferenceManagementHelper
    {
        public static void SetConferenceExists(Mock<IMediator> mediator, Conference conference)
        {
            mediator.Setup(x => x.Send(It.Is<FindConferenceByIdRequest>(x => x.ConferenceId == conference.ConferenceId),
                It.IsAny<CancellationToken>())).ReturnsAsync(conference);
        }

        public static void SetConferenceDoesNotExist(Mock<IMediator> mediator, string conferenceId)
        {
            mediator.Setup(x => x.Send(It.Is<FindConferenceByIdRequest>(x => x.ConferenceId == conferenceId),
                It.IsAny<CancellationToken>())).ThrowsAsync(new ConferenceNotFoundException(conferenceId));
        }

        public static void VerifyAnyFindConferenceById(Mock<IMediator> mediator)
        {
            mediator.Verify(x => x.Send(It.IsAny<FindConferenceByIdRequest>(), It.IsAny<CancellationToken>()));
        }
    }
}
