using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Strive.Core.Errors;

namespace Strive.Extensions
{
    public static class ApiBehaviorExtensions
    {
        public static void UseInvalidModelStateToError(this ApiBehaviorOptions options)
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errorsWithMessage = context.ModelState
                    .Where(x => x.Value.ValidationState == ModelValidationState.Invalid)
                    .ToDictionary(x => x.Key, x => x.Value.Errors.First().ErrorMessage);

                var fieldValidationError = new FieldValidationError(errorsWithMessage);
                return new BadRequestObjectResult(fieldValidationError);
            };
        }
    }
}
