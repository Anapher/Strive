using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Synchronization;
using Strive.Core.Services.Whiteboard.Gateways;

namespace Strive.Core.Services.Whiteboard
{
    public class SynchronizedWhiteboardsProvider : SynchronizedObjectProviderForRoom<SynchronizedWhiteboards>
    {
        private readonly IWhiteboardRepository _repository;

        public SynchronizedWhiteboardsProvider(IMediator mediator, IWhiteboardRepository repository) : base(mediator)
        {
            _repository = repository;
        }

        public override string Id => SynchronizedObjectIds.WHITEBOARDS;

        protected override async ValueTask<SynchronizedWhiteboards> InternalFetchValue(string conferenceId,
            string roomId)
        {
            var whiteboards = await _repository.GetAll(conferenceId, roomId);
            return new SynchronizedWhiteboards(whiteboards.ToDictionary(x => x.Id,
                x => new WhiteboardInfo(x.FriendlyName, x.EveryoneCanEdit)));
        }
    }
}
