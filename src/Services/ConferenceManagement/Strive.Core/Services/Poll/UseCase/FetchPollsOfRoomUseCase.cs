using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Poll.Gateways;
using Strive.Core.Services.Poll.Requests;

namespace Strive.Core.Services.Poll.UseCase
{
    public class FetchPollsOfRoomUseCase : IRequestHandler<FetchPollsOfRoomRequest, IReadOnlyList<Poll>>
    {
        private readonly IPollRepository _repository;

        public FetchPollsOfRoomUseCase(IPollRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<Poll>> Handle(FetchPollsOfRoomRequest request,
            CancellationToken cancellationToken)
        {
            var (conferenceId, roomId) = request;

            var polls = await _repository.GetPollsOfConference(conferenceId);
            return polls.Where(x => x.RoomId == null || x.RoomId == roomId).ToList();
        }
    }
}
