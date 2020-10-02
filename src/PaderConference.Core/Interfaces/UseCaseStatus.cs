using PaderConference.Core.Dto;

namespace PaderConference.Core.Interfaces
{
    public abstract class UseCaseStatus<TResponse> : IUseCaseErrors where TResponse : class
    {
        /// <summary>
        ///     The errors that occurred when executing this UseCase. If empty, the UseCase succeeded
        /// </summary>
        public Error? Error { get; set; }

        /// <summary>
        ///     Returns true if <see cref="Error"/> is not null
        /// </summary>
        public bool HasError => Error != null;

        /// <summary>
        ///     This adds one error to the Errors collection
        /// </summary>
        /// <param name="error">The error that should be added</param>
        protected void SetError(Error error)
        {
            Error = error;
        }

        /// <summary>
        ///     Returns the error: adds the error to the collection and returns default(T).
        /// </summary>
        /// <param name="error">The error that occurred.</param>
        /// <returns>Always return default(T)</returns>
        protected TResponse? ReturnError(Error error)
        {
            SetError(error);
            return default;
        }
    }
}
