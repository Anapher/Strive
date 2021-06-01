using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Poll.Requests;
using Strive.Core.Services.Scenes.Scenes;

namespace Strive.Core.Services.Scenes.Providers
{
    public class PollSceneProvider : ContentSceneProvider
    {
        private readonly IMediator _mediator;

        public PollSceneProvider(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override bool IsProvided(IScene scene)
        {
            return scene is PollScene;
        }

        public override async ValueTask<IEnumerable<IScene>> GetAvailableScenes(string conferenceId, string roomId,
            IReadOnlyList<IScene> sceneStack)
        {
            var polls = await _mediator.Send(new FetchPollsOfRoomRequest(conferenceId, roomId));
            return polls.Select(x => new PollScene(x.Id));
        }

        protected override ValueTask<bool> InternalIsUpdateRequired(string conferenceId, string roomId,
            object synchronizedObject, object? previousValue)
        {
            // Poll scenes are updated manually using UpdateScenesRequest
            // as every poll has an own sync obj and the scene must only be updated
            // on creation and on deletion
            return new(false);
        }
    }
}
