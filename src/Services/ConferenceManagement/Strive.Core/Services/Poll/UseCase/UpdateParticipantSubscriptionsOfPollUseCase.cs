using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Poll.Requests;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Synchronization.Extensions;
using Strive.Core.Services.Synchronization.Requests;

namespace Strive.Core.Services.Poll.UseCase
{
    public class
        UpdateParticipantSubscriptionsOfPollUseCase : IRequestHandler<UpdateParticipantSubscriptionsOfPollRequest>
    {
        private readonly IMediator _mediator;

        public UpdateParticipantSubscriptionsOfPollUseCase(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Unit> Handle(UpdateParticipantSubscriptionsOfPollRequest request,
            CancellationToken cancellationToken)
        {
            var (conferenceId, poll) = request;

            var participantsMap =
                await _mediator.FetchSynchronizedObject<SynchronizedRooms>(conferenceId, SynchronizedRooms.SyncObjId);

            var participants = participantsMap.Participants.Keys;
            if (poll.RoomId != null)
            {
                participants = participantsMap.Participants.Where(x => x.Value == poll.RoomId).Select(x => x.Key)
                    .ToList();
            }

            foreach (var participantId in participants)
            {
                await _mediator.Send(new UpdateSubscriptionsRequest(new Participant(conferenceId, participantId)));
            }

            return Unit.Value;
        }
    }
}
