using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Gateways.Repositories;

namespace PaderConference.Core.Services
{
    /// <summary>
    ///     Inspired by React's useSelector, allows to select a specific property of the conference and issue updated if this
    ///     value changed
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    public class UseConferenceSelector<T> : IAsyncDisposable, IConferenceOptions<T> where T : class
    {
        private readonly string _conferenceId;
        private readonly IConferenceRepo _conferenceRepo;
        private readonly Func<Conference, T> _selectValue;
        private readonly IEqualityComparer<T> _comparer;
        private IAsyncDisposable? _unsubscribeConferenceUpdated;

        /// <summary>
        ///     Initialize a new instance of <see cref="UseConferenceSelector{T}" />. Please note that
        ///     <see cref="InitializeAsync" /> must be called to enable updates.
        /// </summary>
        /// <param name="conferenceId">The conference id</param>
        /// <param name="conferenceRepo">The repository of the conference</param>
        /// <param name="selectValue">A selector for the value</param>
        /// <param name="defaultValue">The initial value</param>
        /// <param name="comparer">
        ///     The comparer to compare the objects to determine if <see cref="Updated" /> should be fired. The
        ///     default equality comparer is used if this value is null
        /// </param>
        public UseConferenceSelector(string conferenceId, IConferenceRepo conferenceRepo,
            Func<Conference, T> selectValue, T defaultValue, IEqualityComparer<T>? comparer = null)
        {
            _conferenceId = conferenceId;
            _conferenceRepo = conferenceRepo;
            _selectValue = selectValue;
            _comparer = comparer ?? EqualityComparer<T>.Default;
            Value = defaultValue;
        }

        public async ValueTask DisposeAsync()
        {
            var unsubscribe = _unsubscribeConferenceUpdated;
            if (unsubscribe != null)
            {
                await unsubscribe.DisposeAsync();
                _unsubscribeConferenceUpdated = null;
            }
        }

        /// <summary>
        ///     The current value. <see cref="InitializeAsync" /> must be called to initialize this property, will be defaultValue
        ///     until then. This property will be updated then.
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        ///     Invoked when the property updated
        /// </summary>
        public event EventHandler<ObjectChangedEventArgs<T>>? Updated;

        /// <summary>
        ///     Initialize this selector. Initialize the <see cref="Value" /> with the current value and listen for changes. Please
        ///     note that <see cref="DisposeAsync" /> must be called
        /// </summary>
        public async Task InitializeAsync()
        {
            var conference = await _conferenceRepo.FindById(_conferenceId);
            if (conference == null)
                throw new InvalidOperationException("The conference could not be found in database.");

            Value = _selectValue(conference);
            _unsubscribeConferenceUpdated =
                await _conferenceRepo.SubscribeConferenceUpdated(_conferenceId, OnConferenceUpdated);
        }

        protected virtual Task OnConferenceUpdated(Conference conference)
        {
            var oldValue = Value;
            var newValue = _selectValue(conference);

            if (!_comparer.Equals(newValue, oldValue))
            {
                Value = newValue;

                Updated?.Invoke(this, new ObjectChangedEventArgs<T>(newValue, oldValue));
            }

            return Task.CompletedTask;
        }
    }
}
