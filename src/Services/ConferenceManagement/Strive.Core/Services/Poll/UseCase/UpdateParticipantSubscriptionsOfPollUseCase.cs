using System.Collections.Generic;
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

            var participantsSubscribed = await _mediator.Send(
                new FetchSubscribedParticipantsRequest(conferenceId, SynchronizedPoll.SyncObjId(poll.Id)));
            var participantsShouldBeSubscribed = await FetchParticipantsThatShouldBeSubscribed(conferenceId, poll);

            foreach (var participant in participantsSubscribed.Union(participantsShouldBeSubscribed))
            {
                await _mediator.Send(new UpdateSubscriptionsRequest(participant));
            }

            return Unit.Value;
        }

        private async Task<IEnumerable<Participant>> FetchParticipantsThatShouldBeSubscribed(string conferenceId,
            Poll poll)
        {
            var participantsMap =
                await _mediator.FetchSynchronizedObject<SynchronizedRooms>(conferenceId, SynchronizedRooms.SyncObjId);

            var participants = participantsMap.Participants.Keys;
            if (poll.RoomId != null)
            {
                participants = participantsMap.Participants.Where(x => x.Value == poll.RoomId).Select(x => x.Key)
                    .ToList();
            }

            return participants.Select(x => new Participant(conferenceId, x));
        }
    }
}
