using PaderConference.Core.Dto;
using PaderConference.Core.Errors;
using PaderConference.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;

namespace PaderConference.Extensions
{
    public static class ResponseExtensions
    {
        private static IImmutableDictionary<string, int> ErrorStatusCodes { get; } =
                new Dictionary<ErrorType, HttpStatusCode>
                {
                    {ErrorType.ValidationError, HttpStatusCode.BadRequest},
                    {ErrorType.Authentication, HttpStatusCode.Unauthorized},
                }.ToImmutableDictionary(x => x.Key.ToString(), x => (int)x.Value);

        public static ActionResult ToActionResult(this IUseCaseErrors status)
        {
            if (!status.HasError)
                return new OkResult();

            return ToActionResult(status.Error!);
        }

        public static ActionResult ToActionResult(this Error error)
        {
            var httpCode = ErrorStatusCodes[error.Type];

            if (error.Fields?.Count > 0)
            {
                error = new Error(error.Type, error.Message, error.Code, ConvertDictionaryKeysToCamelCase(error.Fields)); // clone
            }

            return new ObjectResult(error) { StatusCode = httpCode };
        }

        private static IReadOnlyDictionary<string, TValue> ConvertDictionaryKeysToCamelCase<TValue>(IReadOnlyDictionary<string, TValue> dictionary)
        {
            var newDic = new Dictionary<string, TValue>();
            foreach (var item in dictionary)
            {
                newDic.Add(item.Key.ToCamelCase(), item.Value);
            }

            return newDic;
        }
    }
}