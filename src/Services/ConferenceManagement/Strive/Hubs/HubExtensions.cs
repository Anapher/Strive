using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Strive.Core;
using Strive.Core.Extensions;

namespace Strive.Hubs
{
    public static class HubExtensions
    {
        public static HttpContext GetHttpContext(this Hub hub)
        {
            var httpContext = hub.Context.GetHttpContext();
            if (httpContext == null)
                throw ConferenceError.UnexpectedError("An unexpected error occurred: HttpContext is null")
                    .ToException();

            return httpContext;
        }
    }
}
