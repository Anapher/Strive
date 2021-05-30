using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Poll.Gateways;
using Strive.Core.Services.Poll.Requests;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Synchronization.Extensions;

namespace Strive.Core.Services.Poll.UseCase
{
    public class FetchParticipantPollsUseCase : IRequestHandler<FetchParticipantPollsRequest, IReadOnlyList<Poll>>
    {
        private readonly IPollRepository _repository;
        private readonly IMediator _mediator;

        public FetchParticipantPollsUseCase(IPollRepository repository, IMediator mediator)
        {
            _repository = repository;
            _mediator = mediator;
        }

        public async Task<IReadOnlyList<Poll>> Handle(FetchParticipantPollsRequest request,
            CancellationToken cancellationToken)
        {
            var participant = request.Participant;

            var polls = await _repository.GetPollsOfConference(participant.ConferenceId);

            if (polls.Any(x => x.RoomId != null))
            {
                var participantRoomId = await GetRoomIdOfParticipant(participant);
                polls = polls.Where(x => x.RoomId == null || x.RoomId == participantRoomId).ToList();
            }

            return polls;
        }

        private async ValueTask<string?> GetRoomIdOfParticipant(Participant participant)
        {
            var rooms = await _mediator.FetchSynchronizedObject<SynchronizedRooms>(participant.ConferenceId,
                SynchronizedRooms.SyncObjId);

            rooms.Participants.TryGetValue(participant.Id, out var roomId);
            return roomId;
        }
    }
}
