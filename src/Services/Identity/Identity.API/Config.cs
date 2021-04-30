// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Security.Claims;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Identity.API
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource {Name = "user.info", UserClaims = {ClaimTypes.Role, ClaimTypes.Name}},
            };

        public static Client BuildSpaClient(string host)
        {
            return new()
            {
                ClientId = "spa", AllowedGrantTypes = GrantTypes.Code,
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile,
                    "user.info",
                },
                RedirectUris = {$"{host}/authentication/callback", $"{host}/authentication/silent_callback"},
                PostLogoutRedirectUris = {$"{host}/"},
                AllowedCorsOrigins = {host},
                RequireClientSecret = false,
            };
        }
    }
}
