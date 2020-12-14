namespace PaderConference.Core.Services.Synchronization
{
    /// <summary>
    ///     Manage synchronized objects
    /// </summary>
    public interface ISynchronizationManager
    {
        /// <summary>
        ///     Register a new synchronized object
        /// </summary>
        /// <typeparam name="T">The type of the object that should be synchronized. This type should be immutable.</typeparam>
        /// <param name="name">The name of the synchronized object. This must be unique.</param>
        /// <param name="initialValue">The initial value of the synchronized object</param>
        /// <param name="group">The group that should receive updates to this object</param>
        /// <returns>Return a wrapper class for the synchronized object that allows to issue updates.</returns>
        ISynchronizedObject<T> Register<T>(string name, T initialValue, ParticipantGroup group = ParticipantGroup.All)
            where T : notnull;
    }
}