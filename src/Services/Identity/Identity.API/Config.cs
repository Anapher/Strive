// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer4.Models;

namespace Identity.API
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new[]
            {
                new IdentityResources.OpenId(), new IdentityResources.Profile(),
                new IdentityResource {Name = "user.info", UserClaims = {ClaimTypes.Role, ClaimTypes.Name}},
            };

        public static IEnumerable<ApiScope> ApiScopes => new[] {new ApiScope("conference-management")};

        public static IEnumerable<Client> Clients(string host)
        {
            return new[]
            {
                // m2m client credentials flow client
                new Client
                {
                    ClientId = "m2m.client",
                    ClientName = "Client Credentials Client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = {new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256())},
                    AllowedScopes = {"scope1"},
                },

                // interactive client using code flow + pkce
                new Client
                {
                    ClientId = "spa",
                    ClientSecrets = {new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256())},
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = {$"{host}/authentication/callback", $"{host}/authentication/silent_callback"},
                    PostLogoutRedirectUris = {$"{host}/"},
                    AllowOfflineAccess = true,
                    AllowedScopes = {"openid", "profile", "user.info"},
                    AllowedCorsOrigins = {$"{host}"},
                    RequireClientSecret = false,
                    AlwaysIncludeUserClaimsInIdToken = true,
                },
            };
        }
    }
}