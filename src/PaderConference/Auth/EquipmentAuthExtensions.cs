using System;
using Microsoft.AspNetCore.Authentication;

namespace PaderConference.Auth
{
    public static class EquipmentAuthExtensions
    {
        public static AuthenticationBuilder AddEquipmentAuth(this AuthenticationBuilder builder,
            Action<EquipmentAuthOptions> configureOptions)
        {
            return builder.AddScheme<EquipmentAuthOptions, EquipmentAuthHandler>("Equipment Scheme", "Equipment Auth",
                configureOptions);
        }
    }
}
