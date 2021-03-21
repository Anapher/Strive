using System.Threading.Tasks;
using PaderConference.Core.Services.Media.Gateways;
using PaderConference.Core.Services.Synchronization;

namespace PaderConference.Core.Services.Media
{
    public class SynchronizedMediaStateProvider : SynchronizedObjectProviderForAll<SynchronizedMediaState>
    {
        private readonly IMediaStateRepository _repository;

        public SynchronizedMediaStateProvider(IMediaStateRepository repository)
        {
            _repository = repository;
        }

        public static SynchronizedObjectId SyncObjId = new(SynchronizedObjectIds.MEDIA);

        public override string Id { get; } = SynchronizedObjectIds.MEDIA;

        protected override async ValueTask<SynchronizedMediaState> InternalFetchValue(string conferenceId)
        {
            var streams = await _repository.Get(conferenceId);
            return new SynchronizedMediaState(streams);
        }
    }
}
