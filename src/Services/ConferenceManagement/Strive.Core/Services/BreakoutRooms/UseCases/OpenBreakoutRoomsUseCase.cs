using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Interfaces;
using Strive.Core.Interfaces.Gateways.Repositories;
using Strive.Core.Services.BreakoutRooms.Internal;
using Strive.Core.Services.BreakoutRooms.Requests;
using Strive.Core.Services.Rooms.Requests;

namespace Strive.Core.Services.BreakoutRooms.UseCases
{
    public class OpenBreakoutRoomsUseCase : IRequestHandler<OpenBreakoutRoomsRequest, SuccessOrError<Unit>>
    {
        private readonly IMediator _mediator;

        public OpenBreakoutRoomsUseCase(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<SuccessOrError<Unit>> Handle(OpenBreakoutRoomsRequest request,
            CancellationToken cancellationToken)
        {
            var (_, _, _, assignedRooms, conferenceId) = request;

            BreakoutRoomInternalState? internalState;
            try
            {
                internalState = await _mediator.Send(new ApplyBreakoutRoomRequest(conferenceId, request, null, true),
                    cancellationToken);
            }
            catch (ConcurrencyException)
            {
                return BreakoutRoomError.AlreadyOpen;
            }

            if (assignedRooms?.Length > 0)
            {
                if (internalState == null)
                    throw new InvalidOperationException("The internalState must not be null.");

                try
                {
                    await AssignParticipants(conferenceId, assignedRooms, internalState.OpenedRooms);
                }
                catch (Exception)
                {
                    return BreakoutRoomError.AssigningParticipantsFailed;
                }
            }

            return Unit.Value;
        }

        private async Task AssignParticipants(string conferenceId, string[][] assignedRooms,
            IReadOnlyList<string> openedRooms)
        {
            if (assignedRooms.Length > openedRooms.Count)
                throw new ArgumentException("The assigned rooms must not be greater than the actually opened rooms.");

            var assignments = new List<(string participantId, string roomId)>();

            for (var i = 0; i < assignedRooms.Length; i++)
            {
                var roomId = openedRooms[i];

                foreach (var participantId in assignedRooms[i])
                {
                    assignments.Add((participantId, roomId));
                }
            }

            await _mediator.Send(new SetParticipantRoomRequest(conferenceId, assignments));
        }
    }
}
