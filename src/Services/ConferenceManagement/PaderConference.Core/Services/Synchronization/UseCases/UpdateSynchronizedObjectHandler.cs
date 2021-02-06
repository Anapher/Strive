using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Services.Synchronization.Gateways;
using PaderConference.Core.Services.Synchronization.Notifications;
using PaderConference.Core.Services.Synchronization.Requests;
using PaderConference.Core.Services.Synchronization.UpdateStrategy;

namespace PaderConference.Core.Services.Synchronization.UseCases
{
    public class UpdateSynchronizedObjectHandler<T> : IRequestHandler<UpdateSynchronizedObjectRequest<T>, T>
        where T : notnull
    {
        private readonly ISynchronizedObjectRepository _repository;
        private readonly IMediator _mediator;
        private readonly ILogger<UpdateSynchronizedObjectHandler<T>> _logger;

        public UpdateSynchronizedObjectHandler(ISynchronizedObjectRepository repository, IMediator mediator,
            ILogger<UpdateSynchronizedObjectHandler<T>> logger)
        {
            _repository = repository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<T> Handle(UpdateSynchronizedObjectRequest<T> request, CancellationToken cancellationToken)
        {
            var (valueUpdate, (conferenceId, name, _)) = request;

            _logger.LogDebug("Update synchronized object {name} of conference {conferenceId}", name, conferenceId);

            T newValue;
            T? previousValue;

            switch (valueUpdate)
            {
                case ReplaceValueUpdate<T> replaceValue:
                    previousValue = await _repository.Update(conferenceId, name, replaceValue.Value);
                    newValue = replaceValue.Value;
                    break;
                case PatchValueUpdate<T> patchValue:
                    (previousValue, newValue) = await _repository.Update(conferenceId, name, patchValue.UpdateStateFn);
                    break;
                default:
                    throw new ArgumentException($"The value update of type {valueUpdate.GetType()} is not supported.");
            }

            var notification = new SynchronizedObjectUpdatedNotification(conferenceId, name, newValue, previousValue);
            await _mediator.Publish(notification);

            return newValue;
        }
    }
}
