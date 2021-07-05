using System.Collections.Generic;
using MediatR;

namespace Strive.Core.Services.WhiteboardService.Notifications
{
    public record LiveActionPushedNotification(IReadOnlyList<Participant> Participants, string SenderParticipantId,
        string WhiteboardId, CanvasLiveAction Action) : INotification;
}
