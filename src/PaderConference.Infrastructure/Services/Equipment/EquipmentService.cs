using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Infrastructure.Extensions;
using PaderConference.Infrastructure.Services.Equipment.Dto;

namespace PaderConference.Infrastructure.Services.Equipment
{
    public class EquipmentService : ConferenceService
    {
        private readonly string _conferenceId;
        private readonly ITokenFactory _tokenFactory;
        private readonly ReaderWriterLock _tokenGeneratorLock = new ReaderWriterLock();
        private readonly Dictionary<string, string> _participantToToken = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _tokenToParticipant = new Dictionary<string, string>();

        public EquipmentService(string conferenceId, ITokenFactory tokenFactory)
        {
            _conferenceId = conferenceId;
            _tokenFactory = tokenFactory;
        }

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

        public async ValueTask OnEquipmentConnected(string connectionId)
        {
        }

        public async ValueTask OnEquipmentDisconnected(string connectionId)
        {
        }
    }
}
