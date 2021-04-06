using System.Threading;
using MediatR;
using Moq;
using Strive.Core.Domain.Entities;
using Strive.Core.Services;
using Strive.Core.Services.ConferenceManagement.Requests;

namespace Strive.Core.Tests._TestHelpers
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
