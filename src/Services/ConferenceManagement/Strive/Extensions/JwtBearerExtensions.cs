using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Strive.Extensions
{
    public static class JwtBearerExtensions
    {
        public static void AcceptTokenFromQuery(this JwtBearerOptions options)
        {
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    _ = context.HttpContext.Request.Path;

                    if (!string.IsNullOrEmpty(accessToken))
                        context.Token = accessToken;
                    return Task.CompletedTask;
                },
            };
        }
    }
}
