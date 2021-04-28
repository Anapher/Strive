using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Scenes.Gateways;
using Strive.Core.Services.Scenes.Requests;
using Strive.Core.Services.Scenes.Utilities;

namespace Strive.Core.Services.Scenes.UseCases
{
    public class SetOverwrittenContentSceneUseCase : IRequestHandler<SetOverwrittenContentSceneRequest>
    {
        private readonly IMediator _mediator;
        private readonly ISceneRepository _sceneRepository;

        public SetOverwrittenContentSceneUseCase(IMediator mediator, ISceneRepository sceneRepository)
        {
            _mediator = mediator;
            _sceneRepository = sceneRepository;
        }

        public async Task<Unit> Handle(SetOverwrittenContentSceneRequest request, CancellationToken cancellationToken)
        {
            var (conferenceId, roomId, scene) = request;

            var transaction = new PatchSceneTransaction(_sceneRepository, _mediator);
            await transaction.Handle(conferenceId, roomId, previous => previous with {OverwrittenContent = scene});
            return Unit.Value;
        }
    }
}
