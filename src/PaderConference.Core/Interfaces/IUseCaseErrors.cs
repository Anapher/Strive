using PaderConference.Core.Dto;

namespace PaderConference.Core.Interfaces
{
    public interface IUseCaseErrors
    {
        /// <summary>
        ///     The errors that occurred when executing the use case. If empty, the use case succeeded
        /// </summary>
        Error? Error { get; }

        /// <summary>
        ///     True if error occurred on executing this use case
        /// </summary>
        bool HasError { get; }
    }
}
