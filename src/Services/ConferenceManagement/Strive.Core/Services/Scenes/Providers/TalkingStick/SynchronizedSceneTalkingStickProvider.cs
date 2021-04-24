using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Scenes.Providers.TalkingStick.Gateways;
using Strive.Core.Services.Synchronization;

namespace Strive.Core.Services.Scenes.Providers.TalkingStick
{
    public class
        SynchronizedSceneTalkingStickProvider : SynchronizedObjectProviderForRoom<SynchronizedSceneTalkingStick>
    {
        private readonly ITalkingStickRepository _repository;

        public SynchronizedSceneTalkingStickProvider(IMediator mediator, ITalkingStickRepository repository) :
            base(mediator)
        {
            _repository = repository;
        }

        public override string Id => SynchronizedObjectIds.SCENE_TALKINGSTICK;

        protected override async ValueTask<SynchronizedSceneTalkingStick> InternalFetchValue(string conferenceId,
            string roomId)
        {
            var currentSpeaker = await _repository.GetCurrentSpeaker(conferenceId, roomId);
            var queue = await _repository.FetchQueue(conferenceId, roomId);

            return new SynchronizedSceneTalkingStick(currentSpeaker?.Id, queue);
        }
    }
}
