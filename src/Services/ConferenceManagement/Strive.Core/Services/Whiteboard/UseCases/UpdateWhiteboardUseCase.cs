using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using Strive.Core.Extensions;
using Strive.Core.Services.Synchronization.Requests;
using Strive.Core.Services.Whiteboard.Gateways;
using Strive.Core.Services.Whiteboard.Requests;
using Strive.Core.Services.Whiteboard.Utilities;

namespace Strive.Core.Services.Whiteboard.UseCases
{
    /// <summary>
    ///     Internal action that executes an update on the whiteboard while taking care of the locking, optimistic
    ///     concurrency, versioning and updating the synchronized state. Also, the undo action limitations are enforced by this
    ///     use case.
    /// </summary>
    public class UpdateWhiteboardUseCase : IRequestHandler<UpdateWhiteboardRequest>
    {
        private readonly IMediator _mediator;
        private readonly IWhiteboardRepository _repository;
        private readonly WhiteboardOptions _options;

        public UpdateWhiteboardUseCase(IMediator mediator, IWhiteboardRepository repository,
            IOptions<WhiteboardOptions> options)
        {
            _mediator = mediator;
            _repository = repository;
            _options = options.Value;
        }

        public async Task<Unit> Handle(UpdateWhiteboardRequest request, CancellationToken cancellationToken)
        {
            var (conferenceId, roomId, whiteboardId, action) = request;

            await using (var @lock = await _repository.LockWhiteboard(conferenceId, roomId, whiteboardId))
            {
                var whiteboard = await _repository.Get(conferenceId, roomId, conferenceId);
                if (whiteboard == null)
                    throw WhiteboardError.WhiteboardNotFound.ToException();

                var updated = action(whiteboard);
                updated = EnforceUndoLimitations(updated);
                updated = updated with {Version = whiteboard.Version + 1};

                @lock.HandleLostToken.ThrowIfCancellationRequested();

                await _repository.Create(conferenceId, roomId, updated);

                if (!await RoomUtils.CheckRoomExists(_mediator, conferenceId, roomId))
                {
                    await _repository.Delete(conferenceId, roomId, whiteboard.Id);
                    throw ConferenceError.RoomNotFound.ToException();
                }
            }

            await _mediator.Send(new UpdateSynchronizedObjectRequest(conferenceId,
                SynchronizedWhiteboards.SyncObjId(roomId)));

            return Unit.Value;
        }

        private Whiteboard EnforceUndoLimitations(Whiteboard whiteboard)
        {
            var participantState = whiteboard.ParticipantStates.ToDictionary(x => x.Key, x => x.Value);

            EnforceParticipantIndividualLimit(participantState);
            EnforceGlobalLimit(participantState);

            return whiteboard with {ParticipantStates = participantState.ToImmutableDictionary()};
        }

        private void EnforceGlobalLimit(Dictionary<string, ParticipantWhiteboardState> states)
        {
            var totalHistory = states.Sum(x => x.Value.RedoList.Count + x.Value.UndoList.Count);
            if (totalHistory > _options.MaxUndoHistory)
            {
                var flattenHistory = states.SelectMany(p =>
                    p.Value.UndoList.Select(x => (p.Key, true, x.Version))
                        .Concat(p.Value.RedoList.Select(x => (p.Key, false, x.Version))));

                var entriesToDelete = flattenHistory.OrderBy(x => x.Version)
                    .Take(totalHistory - _options.MaxUndoHistory).ToList();

                // this loop will only execute once as only one action is added at a time
                foreach (var (participantId, isUndo, version) in entriesToDelete)
                {
                    var stateToUpdate = states[participantId];
                    if (isUndo)
                    {
                        stateToUpdate = stateToUpdate with
                        {
                            UndoList = stateToUpdate.UndoList.Where(x => x.Version != version).ToImmutableList(),
                        };
                    }
                    else
                    {
                        stateToUpdate = stateToUpdate with
                        {
                            RedoList = stateToUpdate.RedoList.Where(x => x.Version != version).ToImmutableList(),
                        };
                    }

                    states[participantId] = stateToUpdate;
                }
            }
        }

        private void EnforceParticipantIndividualLimit(Dictionary<string, ParticipantWhiteboardState> states)
        {
            foreach (var (participantId, state) in states.Where(x =>
                x.Value.RedoList.Count + x.Value.UndoList.Count > _options.MaxUndoHistoryForParticipant).ToList())
            {
                var updatedState = state;

                if (state.RedoList.Any())
                {
                    // that should not actually happen, as if the redo list contains any items,
                    // they must've come from the undo list where they should've been removed
                    updatedState = updatedState with {RedoList = ImmutableList<VersionedAction>.Empty};
                }

                if (state.UndoList.Count > _options.MaxUndoHistoryForParticipant)
                {
                    updatedState = updatedState with
                    {
                        UndoList = state.UndoList.TakeLast(_options.MaxUndoHistoryForParticipant).ToImmutableList(),
                    };
                }

                states[participantId] = updatedState;
            }
        }
    }
}
