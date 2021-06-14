using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Synchronization.Requests;
using Strive.Core.Services.Whiteboard.Gateways;
using Strive.Core.Services.Whiteboard.Requests;

namespace Strive.Core.Services.Whiteboard.UseCases
{
    public class CreateWhiteboardUseCase : IRequestHandler<CreateWhiteboardRequest>
    {
        private static readonly IReadOnlyList<string> WhiteboardFriendlyNames =
            new[] {"Da Vinci", "Michelangelo", "Raffaello", "Donatello", "Bob Ross"};

        private readonly IWhiteboardRepository _repository;
        private readonly IMediator _mediator;

        public CreateWhiteboardUseCase(IWhiteboardRepository repository, IMediator mediator)
        {
            _repository = repository;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(CreateWhiteboardRequest request, CancellationToken cancellationToken)
        {
            var (conferenceId, roomId) = request;

            var id = Guid.NewGuid().ToString("N");
            var name = GetFriendlyNameForWhiteboard();
            var whiteboard = new Whiteboard(id, name, false, WhiteboardCanvas.Empty);

            await _repository.Create(conferenceId, roomId, whiteboard);

            await _mediator.Send(new UpdateSynchronizedObjectRequest(conferenceId,
                SynchronizedWhiteboards.SyncObjId(roomId)));

            return Unit.Value;
        }

        private static string GetFriendlyNameForWhiteboard()
        {
            var random = new Random();

            var index = random.Next(WhiteboardFriendlyNames.Count);
            return WhiteboardFriendlyNames[index];
        }
    }
}
