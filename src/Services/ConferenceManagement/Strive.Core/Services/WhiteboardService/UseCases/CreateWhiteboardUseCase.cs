using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Extensions;
using Strive.Core.Services.Synchronization.Requests;
using Strive.Core.Services.WhiteboardService.Gateways;
using Strive.Core.Services.WhiteboardService.Requests;
using Strive.Core.Services.WhiteboardService.Utilities;

namespace Strive.Core.Services.WhiteboardService.UseCases
{
    public class CreateWhiteboardUseCase : IRequestHandler<CreateWhiteboardRequest, string>
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

        public async Task<string> Handle(CreateWhiteboardRequest request, CancellationToken cancellationToken)
        {
            var (conferenceId, roomId) = request;

            var id = Guid.NewGuid().ToString("N");
            var name = GetFriendlyNameForWhiteboard();
            var whiteboard = new Whiteboard(id, name, false, WhiteboardCanvas.Empty,
                ImmutableDictionary<string, ParticipantWhiteboardState>.Empty, 0);

            await _repository.Create(conferenceId, roomId, whiteboard);

            if (!await RoomUtils.CheckRoomExists(_mediator, request.ConferenceId, request.RoomId))
            {
                await _repository.Delete(request.ConferenceId, request.RoomId, whiteboard.Id);
                throw ConferenceError.RoomNotFound.ToException();
            }

            await _mediator.Send(new UpdateSynchronizedObjectRequest(conferenceId,
                SynchronizedWhiteboards.SyncObjId(roomId)));

            return id;
        }

        private static string GetFriendlyNameForWhiteboard()
        {
            var random = new Random();

            var index = random.Next(WhiteboardFriendlyNames.Count);
            return WhiteboardFriendlyNames[index];
        }
    }
}
