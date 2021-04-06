using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Strive.Core.Dto;
using Strive.Core.Errors;
using Strive.Core.Interfaces;
using Strive.Infrastructure.Extensions;

namespace Strive.Extensions
{
    public static class ResponseExtensions
    {
        private static IImmutableDictionary<string, int> ErrorStatusCodes { get; } =
            new Dictionary<ErrorType, HttpStatusCode>
            {
                {ErrorType.BadRequest, HttpStatusCode.BadRequest},
                {ErrorType.Conflict, HttpStatusCode.Conflict},
                {ErrorType.InternalServerError, HttpStatusCode.InternalServerError},
                {ErrorType.Forbidden, HttpStatusCode.Forbidden},
                {ErrorType.NotFound, HttpStatusCode.NotFound},
            }.ToImmutableDictionary(x => x.Key.ToString(), x => (int) x.Value);

        public static ActionResult ToActionResult(this ISuccessOrError status)
        {
            if (status.Success)
                return new OkResult();

            return ToActionResult(status.Error);
        }

        public static ActionResult ToActionResult(this Error error)
        {
            var httpCode = ErrorStatusCodes[error.Type];

            if (error.Fields?.Count > 0)
                error = error with {Fields = ConvertDictionaryKeysToCamelCase(error.Fields)};

            return new ObjectResult(error) { StatusCode = httpCode };
        }

        private static IReadOnlyDictionary<string, TValue> ConvertDictionaryKeysToCamelCase<TValue>(IReadOnlyDictionary<string, TValue> dictionary)
        {
            var newDic = new Dictionary<string, TValue>();
            foreach (var (key, value) in dictionary)
            {
                newDic.Add(key.ToCamelCase(), value);
            }

            return newDic;
        }
    }
}