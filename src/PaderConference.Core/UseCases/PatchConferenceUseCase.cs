using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Dto.Services;
using PaderConference.Core.Dto.UseCaseRequests;
using PaderConference.Core.Dto.UseCaseResponses;
using PaderConference.Core.Dto.Validation;
using PaderConference.Core.Errors;
using PaderConference.Core.Extensions;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Interfaces.UseCases;

namespace PaderConference.Core.UseCases
{
    public class PatchConferenceUseCase : IPatchConferenceUseCase
    {
        private readonly IConferenceRepo _conferenceRepo;
        private readonly ILogger<PatchConferenceUseCase> _logger;

        public PatchConferenceUseCase(IConferenceRepo conferenceRepo, ILogger<PatchConferenceUseCase> logger)
        {
            _conferenceRepo = conferenceRepo;
            _logger = logger;
        }

        public async ValueTask<SuccessOrError<PatchConferenceResponse>> Handle(PatchConferenceRequest message)
        {
            var conference = await _conferenceRepo.FindById(message.ConferenceId);
            if (conference == null)
                return ConferenceError.ConferenceNotFound;

            var config = ConferenceData.FromConference(conference);

            try
            {
                message.Patch.ApplyTo(config);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Json patch failed for conference {conferenceId}", conference.ConferenceId);
                return new FieldValidationError("patch", "An error occurred applyling the json patch");
            }

            var validationResult = new ConferenceDataValidator().Validate(config);
            if (!validationResult.IsValid) return validationResult.ToError();

            conference.Configuration = config.Configuration;
            conference.Permissions = conference.Permissions;

            _logger.LogDebug("Json patch succeeded for conference {conferenceId}. Save to database...",
                conference.ConferenceId);

            await _conferenceRepo.Update(conference);

            return new PatchConferenceResponse(conference);
        }
    }
}
