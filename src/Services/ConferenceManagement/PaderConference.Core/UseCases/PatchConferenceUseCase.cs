using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaderConference.Core.Dto.Services;
using PaderConference.Core.Dto.UseCaseRequests;
using PaderConference.Core.Dto.UseCaseResponses;
using PaderConference.Core.Dto.Validation;
using PaderConference.Core.Errors;
using PaderConference.Core.Extensions;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Interfaces.UseCases;
using PaderConference.Core.Services;
using Polly;

namespace PaderConference.Core.UseCases
{
    public class PatchConferenceUseCase : IPatchConferenceUseCase
    {
        private readonly IConferenceRepo _conferenceRepo;
        private readonly ConcurrencyOptions _options;
        private readonly ILogger<PatchConferenceUseCase> _logger;

        public PatchConferenceUseCase(IConferenceRepo conferenceRepo, IOptions<ConcurrencyOptions> options,
            ILogger<PatchConferenceUseCase> logger)
        {
            _conferenceRepo = conferenceRepo;
            _options = options.Value;
            _logger = logger;
        }

        public async ValueTask<SuccessOrError<PatchConferenceResponse>> Handle(PatchConferenceRequest message)
        {
            return await Policy
                .HandleResult<SuccessOrError<PatchConferenceResponse>>(x =>
                    !x.Success && x.Error.Code == CommonError.ConcurrencyError.Code).RetryAsync(_options.RetryCount,
                    (_, i) => _logger.LogWarning("A concurrency error occurred, retrying for the {i} time.", i))
                .ExecuteAsync(() => HandleInternal(message));
        }

        private async Task<SuccessOrError<PatchConferenceResponse>> HandleInternal(PatchConferenceRequest message)
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
                return new FieldValidationError("patch", "An error occurred applying the json patch");
            }

            var validationResult = new ConferenceDataValidator().Validate(config);
            if (!validationResult.IsValid) return validationResult.ToError();

            conference.Configuration = config.Configuration;
            conference.Permissions = conference.Permissions;

            _logger.LogDebug("Json patch succeeded for conference {conferenceId}. Save to database...",
                conference.ConferenceId);

            var result = await _conferenceRepo.Update(conference);
            return result switch
            {
                OptimisticUpdateResult.ConcurrencyException => CommonError.ConcurrencyError,
                OptimisticUpdateResult.DeletedException => ConferenceError.ConferenceNotFound,
                _ => new PatchConferenceResponse(conference),
            };
        }
    }
}
