using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Strive.Core.Extensions;

namespace Strive.Core.Services.Whiteboard.Utilities
{
    public static class WhiteboardUtils
    {
        private static readonly ParticipantWhiteboardState DefaultState =
            new(ImmutableList<VersionedAction>.Empty, ImmutableList<VersionedAction>.Empty);

        private static IImmutableDictionary<string, ParticipantWhiteboardState> UpdateParticipantState(
            IImmutableDictionary<string, ParticipantWhiteboardState> states, string participantId,
            Func<ParticipantWhiteboardState, ParticipantWhiteboardState> updateAction)
        {
            if (!states.TryGetValue(participantId, out var participantState))
                participantState = DefaultState;

            return states.SetItem(participantId, updateAction(participantState));
        }

        public static IImmutableDictionary<string, ParticipantWhiteboardState> AddParticipantRedoAction(
            IImmutableDictionary<string, ParticipantWhiteboardState> states, VersionedAction action)
        {
            return UpdateParticipantState(states, action.Action.ParticipantId,
                state => state with {RedoList = state.RedoList.Add(action)});
        }

        public static IImmutableDictionary<string, ParticipantWhiteboardState> AddParticipantUndoAction(
            IImmutableDictionary<string, ParticipantWhiteboardState> states, VersionedAction action)
        {
            return UpdateParticipantState(states, action.Action.ParticipantId,
                state => state with {UndoList = state.UndoList.Add(action)});
        }

        public static IImmutableDictionary<string, ParticipantWhiteboardState> AddParticipantUndoActionAndClearRedo(
            IImmutableDictionary<string, ParticipantWhiteboardState> states, VersionedAction action)
        {
            return UpdateParticipantState(states, action.Action.ParticipantId,
                state => state with
                {
                    UndoList = state.UndoList.Add(action), RedoList = ImmutableList<VersionedAction>.Empty,
                });
        }

        public static (VersionedAction, IImmutableDictionary<string, ParticipantWhiteboardState>)
            PopParticipantUndoAction(IImmutableDictionary<string, ParticipantWhiteboardState> states,
                string participantId)
        {
            if (!states.TryGetValue(participantId, out var participantState) || !participantState.UndoList.Any())
            {
                throw WhiteboardError.UndoNotAvailable.ToException();
            }

            var undoAction = participantState.UndoList.Last();
            participantState = participantState with
            {
                UndoList = participantState.UndoList.RemoveAt(participantState.UndoList.Count - 1),
            };

            states = states.SetItem(participantId, participantState);

            return (undoAction, states);
        }

        public static (VersionedAction, IImmutableDictionary<string, ParticipantWhiteboardState>)
            PopParticipantRedoAction(IImmutableDictionary<string, ParticipantWhiteboardState> states,
                string participantId)
        {
            if (!states.TryGetValue(participantId, out var participantState) || !participantState.RedoList.Any())
            {
                throw WhiteboardError.RedoNotAvailable.ToException();
            }

            var redoAction = participantState.RedoList.Last();
            participantState = participantState with
            {
                UndoList = participantState.RedoList.RemoveAt(participantState.UndoList.Count - 1),
            };

            states = states.SetItem(participantId, participantState);

            return (redoAction, states);
        }

        public static string? FindParticipantIdWithLastAction(
            IReadOnlyDictionary<string, ParticipantWhiteboardState> states,
            Func<ParticipantWhiteboardState, IReadOnlyList<VersionedAction>> selector)
        {
            return states.Where(x => selector(x.Value).Any())
                .OrderByDescending(x => selector(x.Value).Select(action => action.Version).Max()).Select(x => x.Key)
                .FirstOrDefault();
        }
    }
}
