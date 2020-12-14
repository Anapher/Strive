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
    }
}