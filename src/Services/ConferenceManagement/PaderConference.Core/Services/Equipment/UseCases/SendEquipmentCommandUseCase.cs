using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Extensions;
using PaderConference.Core.Services.Equipment.Gateways;
using PaderConference.Core.Services.Equipment.Notifications;
using PaderConference.Core.Services.Equipment.Requests;

namespace PaderConference.Core.Services.Equipment.UseCases
{
    public class SendEquipmentCommandUseCase : IRequestHandler<SendEquipmentCommandRequest>
    {
        private readonly IEquipmentConnectionRepository _repository;
        private readonly IMediator _mediator;

        public SendEquipmentCommandUseCase(IEquipmentConnectionRepository repository, IMediator mediator)
        {
            _repository = repository;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(SendEquipmentCommandRequest request, CancellationToken cancellationToken)
        {
            var connection = await _repository.GetConnection(request.Participant, request.ConnectionId);
            if (connection == null)
                throw EquipmentError.NotInitialized.ToException();

            await _mediator.Publish(
                new SendEquipmentCommandNotification(request.Participant, request.ConnectionId, request.Source,
                    request.DeviceId, request.Action), cancellationToken);

            return Unit.Value;
        }
    }
}
