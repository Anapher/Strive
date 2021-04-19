using System.Threading;
using MediatR;
using Moq;
using Strive.Core.Domain.Entities;
using Strive.Core.Services.ConferenceManagement.Requests;
using Strive.Core.Services.Synchronization;
using Strive.Core.Services.Synchronization.Requests;

namespace Strive.Core.Tests._TestHelpers
{
    public static class SynchronizedObjectMediatorExtensions
    {
        public static void SetupSyncObj<T>(this Mock<IMediator> mediator, SynchronizedObjectId syncObjId, T syncObj)
            where T : notnull
        {
            mediator.Setup(x =>
                    x.Send(
                        It.Is<FetchSynchronizedObjectRequest>(request =>
                            request.SyncObjId.ToString().Equals(syncObjId.ToString())), It.IsAny<CancellationToken>()))
                .ReturnsAsync(syncObj);
        }

        public static void SetupConference(this Mock<IMediator> mediator, Conference conference)
        {
            mediator.Setup(x =>
                x.Send(It.Is<FindConferenceByIdRequest>(request => request.ConferenceId == conference.ConferenceId),
                    It.IsAny<CancellationToken>())).ReturnsAsync(conference);
        }
    }
}
