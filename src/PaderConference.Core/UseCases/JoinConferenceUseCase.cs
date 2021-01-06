using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Domain;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Dto.UseCaseRequests;
using PaderConference.Core.Dto.UseCaseResponses;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Interfaces.UseCases;
using PaderConference.Core.Services;
using PaderConference.Core.Services.Equipment;

namespace PaderConference.Core.UseCases
{
    public class JoinConferenceUseCase : IJoinConferenceUseCase
    {
        private readonly IConferenceManager _conferenceManager;
        private readonly IConnectionMapping _connectionMapping;
        private readonly IEnumerable<IConferenceServiceManager> _conferenceServices;
        private readonly IConferenceRepo _conferenceRepo;
        private readonly ILogger<JoinConferenceUseCase> _logger;

        public JoinConferenceUseCase(IConferenceManager conferenceManager, IConnectionMapping connectionMapping,
            IEnumerable<IConferenceServiceManager> conferenceServices, IConferenceRepo conferenceRepo,
            ILogger<JoinConferenceUseCase> logger)
        {
            _conferenceManager = conferenceManager;
            _connectionMapping = connectionMapping;
            _conferenceServices = conferenceServices;
            _conferenceRepo = conferenceRepo;
            _logger = logger;
        }

        public async ValueTask<SuccessOrError<JoinConferenceResponse>> Handle(JoinConferenceRequest message)
        {
            if (!await _conferenceManager.GetIsConferenceOpen(message.ConferenceId))
                if (await _conferenceRepo.FindById(message.ConferenceId) == null)
                    return ConferenceError.ConferenceNotFound;

            // initialize services
            var services =
                await Task.WhenAll(_conferenceServices.Select(x => x.GetService(message.ConferenceId).AsTask()));

            switch (message.Role)
            {
                case PrincipalRoles.Moderator:
                case PrincipalRoles.User:
                case PrincipalRoles.Guest:
                {
                    Participant participant;
                    try
                    {
                        participant = await _conferenceManager.Participate(message.ConferenceId, message.ParticipantId,
                            message.Role, message.Name);
                    }
                    catch (ConferenceNotFoundException)
                    {
                        _logger.LogDebug("Abort connection, conference was not found.");
                        return ConferenceError.ConferenceNotFound;
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Unexpected exception occurred.");
                        return ConferenceError.UnexpectedError(e.Message);
                    }

                    if (!_connectionMapping.Add(message.ConnectionId, participant))
                    {
                        await _conferenceManager.RemoveParticipant(participant);

                        _logger.LogError("Participant {participantId} could not be added to connection mapping.",
                            participant.ParticipantId);
                        return ConferenceError.UnexpectedError("Participant could not be added to connection mapping.");
                    }

                    // initialize services
                    try
                    {
                        await ExecuteServiceAction(services, service => service.OnClientConnected(participant).AsTask(),
                            message.CancellationToken);

                        await message.EnableParticipantMessaging();

                        await ExecuteServiceAction(services,
                            service => service.InitializeParticipant(participant).AsTask(), message.CancellationToken);
                    }
                    catch (Exception e)
                    {
                        // if an error occurred, notify services about client disconnect

                        _logger.LogError(e,
                            "An exception occurred on executing on initializing services for participant {participant}",
                            participant.ParticipantId);
                        try
                        {
                            await Task.WhenAll(services.Select(x => x.OnClientDisconnected(participant).AsTask()));
                        }
                        catch (Exception e2)
                        {
                            _logger.LogError(e2,
                                "An error occurred on disconnecting participant after OnClientConnected failed.");
                            // ignored
                        }

                        return ConferenceError.InternalError("An error occurred on initializing participant.");
                    }

                    return new JoinConferenceResponse(participant);
                }
                case PrincipalRoles.Equipment:
                {
                    if (!_conferenceManager.TryGetParticipant(message.ConferenceId, message.ParticipantId,
                        out var participant))
                        return ConferenceError.UnexpectedError("Participant is not connected to this conference.");

                    if (!_connectionMapping.Add(message.ConnectionId, participant, true))
                    {
                        _logger.LogError("Participant {participantId} could not be added to connection mapping.",
                            participant.ParticipantId);
                        return ConferenceError.UnexpectedError("Participant could not be added to connection mapping.");
                    }

                    // only tell the equipment service about the new equipment
                    var equipmentService = services.OfType<EquipmentService>().First();
                    await equipmentService.OnEquipmentConnected(participant, message.ConnectionId);

                    return new JoinConferenceResponse(participant);
                }
                default:
                    return ConferenceError.UnexpectedError($"Invalid role: {message.Role}");
            }
        }

        private async Task ExecuteServiceAction(IEnumerable<IConferenceService> services,
            Func<IConferenceService, Task> action, CancellationToken token)
        {
            foreach (var service in services)
            {
                token.ThrowIfCancellationRequested();

                try
                {
                    await action(service);
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, "An error occurred on invoking {action} on service {service}", action,
                        service);
                    throw;
                }
            }
        }
    }
}
