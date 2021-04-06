using System.Threading.Tasks;
using Strive.Core.Services.Media.Gateways;
using Strive.Core.Services.Synchronization;

namespace Strive.Core.Services.Media
{
    public class SynchronizedMediaStateProvider : SynchronizedObjectProviderForAll<SynchronizedMediaState>
    {
        private readonly IMediaStateRepository _repository;

        public SynchronizedMediaStateProvider(IMediaStateRepository repository)
        {
            _repository = repository;
        }


        public override string Id { get; } = SynchronizedMediaState.SyncObjId.Id;

        protected override async ValueTask<SynchronizedMediaState> InternalFetchValue(string conferenceId)
        {
            var streams = await _repository.Get(conferenceId);
            return new SynchronizedMediaState(streams);
        }
    }
}
