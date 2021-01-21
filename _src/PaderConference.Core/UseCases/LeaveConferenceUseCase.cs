using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Domain;
using PaderConference.Core.Dto.UseCaseRequests;
using PaderConference.Core.Dto.UseCaseResponses;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Interfaces.UseCases;
using PaderConference.Core.Services;
using PaderConference.Core.Services.Equipment;

namespace PaderConference.Core.UseCases
{
    public class LeaveConferenceUseCase : ILeaveConferenceUseCase
    {
        private readonly IConnectionMapping _connectionMapping;
        private readonly IConferenceManager _conferenceManager;
        private readonly IEnumerable<IConferenceServiceManager> _conferenceServices;
        private readonly ILogger<LeaveConferenceUseCase> _logger;

        public LeaveConferenceUseCase(IConnectionMapping connectionMapping, IConferenceManager conferenceManager,
            IEnumerable<IConferenceServiceManager> conferenceServices, ILogger<LeaveConferenceUseCase> logger)
        {
            _connectionMapping = connectionMapping;
            _conferenceManager = conferenceManager;
            _conferenceServices = conferenceServices;
            _logger = logger;
        }

        public async ValueTask<SuccessOrError<LeaveConferenceResponse>> Handle(LeaveConferenceRequest message)
        {
            _logger.LogDebug("Connection {connectionId} leaves conference...", message.ConnectionId);

            if (!_connectionMapping.Connections.TryGetValue(message.ConnectionId, out var participant))
            {
                _logger.LogWarning("Connection {connectionId} did not exist in connection mapping. skip.",
                    message.ConnectionId);
                return new LeaveConferenceResponse();
            }

            try
            {
                var conferenceId = _conferenceManager.GetConferenceOfParticipant(participant);
                if (participant.Role == PrincipalRoles.Equipment)
                {
                    var equipmentService = await _conferenceServices
                        .OfType<IConferenceServiceManager<EquipmentService>>().First().GetService(conferenceId);

                    await equipmentService.OnEquipmentDisconnected(participant, message.ConnectionId);
                    return new LeaveConferenceResponse();
                }

                // important for participants list, else the disconnected participant will still be in the list
                await _conferenceManager.RemoveParticipant(participant);

                // Todo: close conference if it was the last participant and some time passed
                await Task.WhenAll(_conferenceServices.Select(async x =>
                    await (await x.GetService(conferenceId)).OnClientDisconnected(participant)));

                return new LeaveConferenceResponse();
            }
            finally
            {
                _connectionMapping.Remove(message.ConnectionId);
            }
        }
    }
}
