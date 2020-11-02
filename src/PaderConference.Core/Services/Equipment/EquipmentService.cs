using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Dto;
using PaderConference.Core.Extensions;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Services.Equipment.Data;
using PaderConference.Core.Services.Equipment.Dto;
using PaderConference.Core.Signaling;

namespace PaderConference.Core.Services.Equipment
{
    public class EquipmentService : ConferenceService
    {
        private readonly string _conferenceId;
        private readonly ITokenFactory _tokenFactory;
        private readonly ISignalMessenger _hubContext;
        private readonly IConnectionMapping _connectionMapping;
        private readonly ReaderWriterLock _tokenGeneratorLock = new ReaderWriterLock();
        private readonly Dictionary<string, string> _participantToToken = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _tokenToParticipant = new Dictionary<string, string>();

        private readonly ConcurrentDictionary<string, ParticipantEquipment> _participantEquipment =
            new ConcurrentDictionary<string, ParticipantEquipment>();

        public EquipmentService(string conferenceId, ITokenFactory tokenFactory, ISignalMessenger hubContext,
            IConnectionMapping connectionMapping)
        {
            _conferenceId = conferenceId;
            _tokenFactory = tokenFactory;
            _hubContext = hubContext;
            _connectionMapping = connectionMapping;
        }

        // TODO: on participant disconnect, disconnect equipment

        public ValueTask<EquipmentAuthResponse> AuthenticateEquipment(string token)
        {
            lock (_tokenGeneratorLock)
            {
                if (_tokenToParticipant.TryGetValue(token, out var participantId))
                    return new EquipmentAuthResponse(participantId).ToValueTask();

                return new EquipmentAuthResponse().ToValueTask();
            }
        }

        public ValueTask<string> GetEquipmentToken(IServiceMessage message)
        {
            // check if the participant already has an equipment token
            lock (_tokenGeneratorLock)
            {
                if (_participantToToken.TryGetValue(message.Participant.ParticipantId, out var token))
                    return token.ToValueTask();
            }

            // generate a new token
            var newToken = _tokenFactory.GenerateToken(16);
            lock (_tokenGeneratorLock)
            {
                // check if the participant got a new token between the locks
                if (_participantToToken.TryGetValue(message.Participant.ParticipantId, out var token))
                    return token.ToValueTask();

                if (_tokenToParticipant.ContainsKey(newToken))
                    return GetEquipmentToken(message); // when our random would be broken (or we are a little unlucky)

                _participantToToken[message.Participant.ParticipantId] = newToken;
                _tokenToParticipant[newToken] = message.Participant.ParticipantId;

                return newToken.ToValueTask();
            }
        }

        public async ValueTask RegisterEquipment(IServiceMessage<RegisterEquipmentRequestDto> message)
        {
            if (_participantEquipment.TryGetValue(message.Participant.ParticipantId, out var equipment))
            {
                await equipment.RegisterEquipment(message.ConnectionId, message.Payload);
                await UpdateParticipantEquipment(message.Participant, equipment);
            }
        }

        public async ValueTask SendEquipmentCommand(IServiceMessage<EquipmentCommand> message)
        {
            if (_participantEquipment.TryGetValue(message.Participant.ParticipantId, out var equipment))
            {
                var connection = equipment.GetConnection(message.Payload.EquipmentId);
                if (connection == null)
                {
                    await message.ResponseError(EquipmentError.NotFound);
                    return;
                }

                await _hubContext.SendToConnectionAsync(connection.ConnectionId,
                    CoreHubMessages.Response.OnEquipmentCommand, message.Payload);
            }
        }

        public async ValueTask EquipmentErrorOccurred(IServiceMessage<Error> message)
        {
            if (_connectionMapping.ConnectionsR.TryGetValue(message.Participant, out var connections))
                await _hubContext.SendToConnectionAsync(connections.MainConnectionId, CoreHubMessages.Response.OnError,
                    message.Payload);
        }

        public async ValueTask EquipmentUpdateStatus(IServiceMessage<Dictionary<string, UseMediaStateInfo>> message)
        {
            if (_participantEquipment.TryGetValue(message.Participant.ParticipantId, out var equipment))
            {
                equipment.UpdateStatus(message.ConnectionId, message.Payload);
                await UpdateParticipantEquipment(message.Participant, equipment);
            }
        }

        public async ValueTask OnEquipmentConnected(Participant participant, string connectionId)
        {
            var equipment = _participantEquipment.GetOrAdd(participant.ParticipantId, _ => new ParticipantEquipment());
            await equipment.OnEquipmentConnected(connectionId);
            await UpdateParticipantEquipment(participant, equipment);
        }

        public async ValueTask OnEquipmentDisconnected(Participant participant, string connectionId)
        {
            if (_participantEquipment.TryGetValue(participant.ParticipantId, out var equipment))
            {
                await equipment.OnEquipmentDisconnected(connectionId);
                await UpdateParticipantEquipment(participant, equipment);
            }
        }

        private async ValueTask UpdateParticipantEquipment(Participant participant, ParticipantEquipment equipment)
        {
            var status = equipment.GetStatus();
            var connectionId = _connectionMapping.ConnectionsR[participant].MainConnectionId;

            await _hubContext.SendToConnectionAsync(connectionId, CoreHubMessages.Response.OnEquipmentUpdated, status);
        }
    }
}
