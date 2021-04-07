using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Strive.Core.Errors;
using Strive.Core.Extensions;
using Strive.Core.Interfaces;
using Strive.Core.Interfaces.Gateways;
using Strive.Core.Services.ConferenceManagement.Gateways;
using Strive.Core.Services.ConferenceManagement.Requests;

namespace Strive.Core.Services.ConferenceManagement.UseCases
{
    public class PatchConferenceRequestHandler : IRequestHandler<PatchConferenceRequest, SuccessOrError<Unit>>
    {
        private readonly IConferenceRepo _conferenceRepo;
        private readonly ConcurrencyOptions _options;
        private readonly ILogger<PatchConferenceRequestHandler> _logger;

        public PatchConferenceRequestHandler(IConferenceRepo conferenceRepo, IOptions<ConcurrencyOptions> options,
            ILogger<PatchConferenceRequestHandler> logger)
        {
            _conferenceRepo = conferenceRepo;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<SuccessOrError<Unit>> Handle(PatchConferenceRequest request,
            CancellationToken cancellationToken)
        {
            // handle optimistic concurrency
            return await Policy
                .HandleResult<SuccessOrError<Unit>
                >(x => !x.Success && x.Error.Code == CommonError.ConcurrencyError.Code)
                .RetryAsync(_options.RetryCount,
                    (_, i) => _logger.LogWarning("A concurrency error occurred, retrying for the {i}. time.", i))
                .ExecuteAsync(() => HandleInternal(request, cancellationToken));
        }

        private async Task<SuccessOrError<Unit>> HandleInternal(PatchConferenceRequest request,
            CancellationToken cancellationToken)
        {
            var (conferenceId, patch) = request;

            cancellationToken.ThrowIfCancellationRequested();

            var conference = await _conferenceRepo.FindById(conferenceId);
            if (conference == null)
                return ConferenceError.ConferenceNotFound;

            var config = ConferenceData.FromConference(conference);

            try
            {
                patch.ApplyTo(config);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Json patch failed for conference {conferenceId}", conference.ConferenceId);
                return new FieldValidationError("patch", "An error occurred applying the json patch");
            }

            var validationResult = new ConferenceDataValidator().Validate(config);
            if (!validationResult.IsValid) return validationResult.ToError();

            conference.Configuration = config.Configuration;
            conference.Permissions = config.Permissions;

            _logger.LogDebug("Json patch succeeded for conference {conferenceId}. Save to database...",
                conference.ConferenceId);

            var result = await _conferenceRepo.Update(conference);
            return result switch
            {
                OptimisticUpdateResult.ConcurrencyException => CommonError.ConcurrencyError,
                OptimisticUpdateResult.DeletedException => ConferenceError.ConferenceNotFound,
                _ => Unit.Value,
            };
        }
    }
}
