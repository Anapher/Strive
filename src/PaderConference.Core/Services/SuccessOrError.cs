using System;
using System.Diagnostics.CodeAnalysis;
using PaderConference.Core.Dto;

namespace PaderConference.Core.Services
{
    public interface ISuccessOrError
    {
        [MemberNotNullWhen(false, nameof(Error))]
        bool Success { get; }

        public Error? Error { get; }
    }

    public class SuccessOrError : ISuccessOrError
    {
        [MemberNotNullWhen(false, nameof(Error))]
        public bool Success => Error == null;

        public Error? Error { get; init; }

        public static SuccessOrError Succeeded = new();

        public static SuccessOrError Failed(Error error)
        {
            return new() {Error = error};
        }

        public static implicit operator SuccessOrError(Error error)
        {
            return Failed(error);
        }
    }

    public class SuccessOrError<T> : ISuccessOrError
    {
        public SuccessOrError(T response)
        {
            Response = response;
        }

        public SuccessOrError(Error error)
        {
            Error = error;
        }

        public SuccessOrError()
        {
        }

#pragma warning disable CS8775 // Member must have a non-null value when exiting in some condition.
        [MemberNotNullWhen(false, nameof(Error))]
        [MemberNotNullWhen(true, nameof(Response))]
        public bool Success => Error == null;
#pragma warning restore CS8775 // Member must have a non-null value when exiting in some condition.

        public Error? Error { get; init; }

        public T? Response { get; init; }

        public static SuccessOrError<T> Succeeded(T response)
        {
            return new() {Response = response};
        }

        public static SuccessOrError<T> Failed(Error error)
        {
            return new() {Error = error};
        }

        public static implicit operator SuccessOrError<T>(Error error)
        {
            return Failed(error);
        }

        public static implicit operator SuccessOrError<T>(T result)
        {
            if (result is Error)
                throw new InvalidOperationException("Cannot implicitly convert an error to a success result");

            return Succeeded(result);
        }
    }
}
