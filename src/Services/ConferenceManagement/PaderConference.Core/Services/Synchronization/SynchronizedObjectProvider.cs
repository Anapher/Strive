using System;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services.Synchronization.Requests;
using PaderConference.Core.Services.Synchronization.UpdateStrategy;

namespace PaderConference.Core.Services.Synchronization
{
    public abstract class SynchronizedObjectProvider<T> : ISynchronizedObjectProvider<T> where T : class
    {
        private readonly IMediator _mediator;

        protected SynchronizedObjectProvider(IMediator mediator)
        {
            _mediator = mediator;
        }

        public abstract ValueTask<T> GetInitialValue(string conferenceId);

        public virtual string Name { get; } = typeof(T).Name;

        public virtual ParticipantGroup TargetGroup { get; } = ParticipantGroup.All;

        public async ValueTask<T> Update(string conferenceId, T newValue)
        {
            var metadata = GetMetadata(conferenceId);
            var result =
                await _mediator.Send(
                    new UpdateSynchronizedObjectRequest<T>(new ReplaceValueUpdate<T>(newValue), metadata));

            return result;
        }

        public async ValueTask<T> Update(string conferenceId, Func<T, T> updateStateFn)
        {
            var currentVal = await GetInitialValue(conferenceId);

            T WrapUpdateFn(T? o)
            {
                return updateStateFn(o ?? currentVal);
            }

            var metadata = GetMetadata(conferenceId);
            var result = await _mediator.Send(
                new UpdateSynchronizedObjectRequest<T>(new PatchValueUpdate<T>(WrapUpdateFn), metadata));

            return result;
        }

        private SynchronizedObjectMetadata GetMetadata(string conferenceId)
        {
            return new(conferenceId, Name, TargetGroup);
        }
    }
}
