namespace PaderConference.Core.Services.Synchronization
{
    /// <summary>
    ///     A wrapper for an object that will be synchronized
    /// </summary>
    public interface ISynchronizedObject
    {
        /// <summary>
        ///     Get the current value of the object
        /// </summary>
        /// <returns>Return the current value</returns>
        object GetCurrent();
    }
}