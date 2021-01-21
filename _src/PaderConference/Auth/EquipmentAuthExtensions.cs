using System;
using Microsoft.AspNetCore.Authentication;

namespace PaderConference.Auth
{
    public static class EquipmentAuthExtensions
    {
        public const string EquipmentAuthScheme = "Equipment Scheme";

        public static AuthenticationBuilder AddEquipmentAuth(this AuthenticationBuilder builder,
            Action<EquipmentAuthOptions> configureOptions)
        {
            return builder.AddScheme<EquipmentAuthOptions, EquipmentAuthHandler>(EquipmentAuthScheme, "Equipment Auth",
                configureOptions);
        }
    }
}
