using System;
using System.Threading.Tasks;

namespace PaderConference.Core.Services.Synchronization
{
    /// <summary>
    ///     A wrapper for an object that will be synchronized
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISynchronizedObject<T> : ISynchronizedObject
    {
        /// <summary>
        ///     The current value of this synchronized object
        /// </summary>
        T Current { get; }

        /// <summary>
        ///     Update the synchronized object and send the new version to the participant group
        /// </summary>
        /// <param name="newValue">The new value of the synchronized object</param>
        ValueTask Update(T newValue);

        /// <summary>
        ///     Update the synchronized object thread safe with the data from the previous state
        /// </summary>
        /// <param name="updateStateFn">A delegate to create a new state that may also depend on the previous state.</param>
        /// <returns>Return the state that was created in <see cref="updateStateFn" />.</returns>
        ValueTask<T> Update(Func<T, T> updateStateFn);
    }
}