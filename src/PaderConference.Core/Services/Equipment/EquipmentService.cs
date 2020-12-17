using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
        private readonly ITokenFactory _tokenFactory;
        private readonly ISignalMessenger _messenger;
        private readonly IConnectionMapping _connectionMapping;
        private readonly ILogger<EquipmentService> _logger;
        private readonly ReaderWriterLock _tokenGeneratorLock = new();
        private readonly Dictionary<string, string> _participantToToken = new();
        private readonly Dictionary<string, string> _tokenToParticipant = new();

        private readonly ConcurrentDictionary<string, ParticipantEquipment> _participantEquipment = new();

        public EquipmentService(ITokenFactory tokenFactory, ISignalMessenger messenger,
            IConnectionMapping connectionMapping, ILogger<EquipmentService> logger)
        {
            _tokenFactory = tokenFactory;
            _messenger = messenger;
            _connectionMapping = connectionMapping;
            _logger = logger;
        }

        // TODO: on participant disconnect, disconnect equipment

        public async ValueTask<SuccessOrError<string>> AuthenticateEquipment(string token)
        {
            using var _ = _logger.BeginMethodScope();

            _logger.LogDebug("Try to authenticate with token {token}", token);

            lock (_tokenGeneratorLock)
            {
                if (_tokenToParticipant.TryGetValue(token, out var participantId))
                {
                    _logger.LogInformation("Authentication succeeded, participantId: {id}", participantId);
                    return participantId;
                }

                _logger.LogDebug("Authentication failed");
                return CommonError.ParticipantNotFound;
            }
        }

        public ValueTask<SuccessOrError<string>> GetEquipmentToken(IServiceMessage message)
        {
            using var _ = _logger.BeginMethodScope(message.GetScopeData());

            return InternalGetEquipmentToken(message).ToValueTask();
        }

        private SuccessOrError<string> InternalGetEquipmentToken(IServiceMessage message)
        {
            // check if the participant already has an equipment token
            lock (_tokenGeneratorLock)
            {
                if (_participantToToken.TryGetValue(message.Participant.ParticipantId, out var token))
                {
                    _logger.LogDebug("Participant already has a token, return {token}", token);
                    return token;
                }
            }

            _logger.LogDebug("Generate new token...");

            // generate a new token
            var newToken = _tokenFactory.GenerateToken(16);
            lock (_tokenGeneratorLock)
            {
                // check if the participant got a new token between the locks
                if (_participantToToken.TryGetValue(message.Participant.ParticipantId, out var token))
                {
                    _logger.LogDebug("Race condition, discard generated token and return existing one.");
                    return token;
                }

                if (_tokenToParticipant.ContainsKey(newToken))
                {
                    _logger.LogError(
                        "Generated a token that already exists. Generated token: {token}, _tokenToParticipant: {@tokenToParticipant}",
                        token, _tokenToParticipant);
                    return
                        InternalGetEquipmentToken(
                            message); // when our random would be broken (or we are a little unlucky)
                }

                _logger.LogDebug("Remember new token and return");

                _participantToToken[message.Participant.ParticipantId] = newToken;
                _tokenToParticipant[newToken] = message.Participant.ParticipantId;

                return newToken;
            }
        }

        public async ValueTask<SuccessOrError> RegisterEquipment(IServiceMessage<RegisterEquipmentRequestDto> message)
        {
            if (_participantEquipment.TryGetValue(message.Participant.ParticipantId, out var equipment))
            {
                await equipment.RegisterEquipment(message.ConnectionId, message.Payload);
                await UpdateParticipantEquipment(message.Participant, equipment);
            }

            return SuccessOrError.Succeeded;
        }

        public async ValueTask<SuccessOrError> SendEquipmentCommand(IServiceMessage<EquipmentCommand> message)
        {
            using var _ = _logger.BeginMethodScope(message.GetScopeData());

            if (_participantEquipment.TryGetValue(message.Participant.ParticipantId, out var equipment))
            {
                var connection = equipment.GetConnection(message.Payload.EquipmentId);
                if (connection == null)
                {
                    _logger.LogWarning(
                        "Sending command to equipment failed, equipment {eqId} was not found for participant {pid}",
                        message.Payload.EquipmentId, message.Participant.ParticipantId);
                    return EquipmentError.NotFound;
                }

                _logger.LogInformation("Send equipment command to {connId}, command: {@cmd}", connection.ConnectionId,
                    message.Payload);

                await _messenger.SendToConnectionAsync(connection.ConnectionId,
                    CoreHubMessages.Response.OnEquipmentCommand, message.Payload);
            }
            else
            {
                _logger.LogWarning("Participant {pid} was not found.", message.Participant.ParticipantId);
            }

            return SuccessOrError.Succeeded;
        }

        public async ValueTask<SuccessOrError> EquipmentErrorOccurred(IServiceMessage<Error> message)
        {
            using var _ = _logger.BeginMethodScope(message.GetScopeData());

            _logger.LogDebug("Equipment error occurred: {@error}", message.Payload);

            if (_connectionMapping.ConnectionsR.TryGetValue(message.Participant.ParticipantId, out var connections))
                await _messenger.SendToConnectionAsync(connections.MainConnectionId, CoreHubMessages.Response.OnError,
                    message.Payload);
            else
                _logger.LogWarning("The participant was not found.");

            return SuccessOrError.Succeeded;
        }

        public async ValueTask<SuccessOrError> EquipmentUpdateStatus(
            IServiceMessage<Dictionary<string, UseMediaStateInfo>> message)
        {
            using var _ = _logger.BeginMethodScope(message.GetScopeData());

            _logger.LogDebug("Update equipment status: {@payload}", message.Payload);

            if (_participantEquipment.TryGetValue(message.Participant.ParticipantId, out var equipment))
            {
                equipment.UpdateStatus(message.ConnectionId, message.Payload);
                await UpdateParticipantEquipment(message.Participant, equipment);
            }
            else
            {
                _logger.LogWarning("The participant was not found.");
            }

            return SuccessOrError.Succeeded;
        }

        public async ValueTask OnEquipmentConnected(Participant participant, string connectionId)
        {
            using var _ = _logger.BeginMethodScope(new Dictionary<string, object>
            {
                {"participantId", participant.ParticipantId}, {"connectionId", connectionId},
            });

            _logger.LogDebug("Connect equipment from {connId}", connectionId);

            var equipment = _participantEquipment.GetOrAdd(participant.ParticipantId, _ => new ParticipantEquipment());
            await equipment.OnEquipmentConnected(connectionId);
            await UpdateParticipantEquipment(participant, equipment);
        }

        public async ValueTask OnEquipmentDisconnected(Participant participant, string connectionId)
        {
            using var _ = _logger.BeginMethodScope(new Dictionary<string, object>
            {
                {"participantId", participant.ParticipantId}, {"connectionId", connectionId},
            });

            _logger.LogDebug("Disconnect equipment from {connId}", connectionId);


            if (_participantEquipment.TryGetValue(participant.ParticipantId, out var equipment))
            {
                await equipment.OnEquipmentDisconnected(connectionId);
                await UpdateParticipantEquipment(participant, equipment);
            }
            else
            {
                _logger.LogDebug("Equipment could not be disconnected as it was not found.");
            }
        }

        private async ValueTask UpdateParticipantEquipment(Participant participant, ParticipantEquipment equipment)
        {
            var status = equipment.GetStatus();
            _logger.LogDebug("Update equipment for participant {id}: {@status}", participant.ParticipantId, status);

            var connectionId = _connectionMapping.ConnectionsR[participant.ParticipantId].MainConnectionId;
            await _messenger.SendToConnectionAsync(connectionId, CoreHubMessages.Response.OnEquipmentUpdated, status);
        }
    }
}
