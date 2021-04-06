using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Strive.Core.Services.Synchronization.Gateways;
using Strive.Core.Services.Synchronization.Notifications;
using Strive.Core.Services.Synchronization.Requests;

namespace Strive.Core.Services.Synchronization.UseCases
{
    public class UpdateSynchronizedObjectUseCase : IRequestHandler<UpdateSynchronizedObjectRequest>
    {
        private readonly IEnumerable<ISynchronizedObjectProvider> _providers;
        private readonly ISynchronizedObjectRepository _synchronizedObjectRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<UpdateSynchronizedObjectUseCase> _logger;

        public UpdateSynchronizedObjectUseCase(IEnumerable<ISynchronizedObjectProvider> providers,
            ISynchronizedObjectRepository synchronizedObjectRepository, IMediator mediator,
            ILogger<UpdateSynchronizedObjectUseCase> logger)
        {
            _providers = providers;
            _synchronizedObjectRepository = synchronizedObjectRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateSynchronizedObjectRequest request, CancellationToken cancellationToken)
        {
            var (conferenceId, synchronizedObjectId) = request;

            var provider = GetProvider(synchronizedObjectId);
            var subscribedParticipants =
                await _mediator.Send(new FetchSubscribedParticipantsRequest(conferenceId, synchronizedObjectId),
                    cancellationToken);

            _logger.LogDebug("Update synchronized object {syncObj} ({count} subscribers)", synchronizedObjectId,
                subscribedParticipants.Count);

            if (!subscribedParticipants.Any()) return Unit.Value;

            var newValue = await provider.FetchValue(conferenceId, synchronizedObjectId);
            var previousValue = await _synchronizedObjectRepository.Create(conferenceId,
                synchronizedObjectId.ToString(), newValue, provider.Type);

            if (Equals(newValue, previousValue)) return Unit.Value;

            await _mediator.Publish(
                new SynchronizedObjectUpdatedNotification(subscribedParticipants, synchronizedObjectId.ToString(),
                    newValue, previousValue), cancellationToken);

            return Unit.Value;
        }

        private ISynchronizedObjectProvider GetProvider(SynchronizedObjectId synchronizedObjectId)
        {
            var provider = _providers.FirstOrDefault(x => x.Id == synchronizedObjectId.Id);
            if (provider == null)
                throw new InvalidOperationException($"There was no provider registered for {synchronizedObjectId.Id}.");

            return provider;
        }
    }
}
