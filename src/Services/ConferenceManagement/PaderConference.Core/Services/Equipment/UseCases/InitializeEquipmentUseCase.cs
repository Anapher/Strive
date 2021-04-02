using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Extensions;
using PaderConference.Core.Services.ConferenceControl.Requests;
using PaderConference.Core.Services.Equipment.Gateways;
using PaderConference.Core.Services.Equipment.Requests;
using PaderConference.Core.Services.Synchronization.Requests;

namespace PaderConference.Core.Services.Equipment.UseCases
{
    public class InitializeEquipmentUseCase : IRequestHandler<InitializeEquipmentRequest>
    {
        private readonly IEquipmentConnectionRepository _repository;
        private readonly IMediator _mediator;

        public InitializeEquipmentUseCase(IEquipmentConnectionRepository repository, IMediator mediator)
        {
            _repository = repository;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(InitializeEquipmentRequest request, CancellationToken cancellationToken)
        {
            var devices = request.Devices.ToDictionary(x => x.DeviceId, x => x);
            var connection = new EquipmentConnection(request.ConnectionId, request.Name, devices,
                ImmutableDictionary<string, UseMediaStateInfo>.Empty);

            await _repository.SetConnection(request.Participant, connection);

            var isJoined = await _mediator.Send(new CheckIsParticipantJoinedRequest(request.Participant));
            if (!isJoined)
            {
                await _repository.RemoveConnection(request.Participant, connection.ConnectionId);
                throw EquipmentError.ParticipantNotJoined.ToException();
            }

            await _mediator.Send(new UpdateSynchronizedObjectRequest(request.Participant.ConferenceId,
                SynchronizedEquipment.SyncObjId(request.Participant.Id)));

            return Unit.Value;
        }
    }
}
