using System.Security.Claims;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;

namespace IdentityServer
{
    public class ConfigureIdentityServer
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile(),
                new IdentityResources.Phone(),
                new IdentityResources.Address(),

                new IdentityResource(
                    name: "roleIdentity",
                    displayName: "Username and role",
                    claimTypes: new[] { "role", "username" }
                )
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            ApiResource caseResource = new ApiResource(
                    name: "caseApi"
            );
            caseResource.UserClaims = new[] { "role" };
            return new [] { caseResource };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
                {
                    new Client
                    {
                        ClientId = "andrej.client",
                        ClientName = "AndrejGorgioski.com",
                        ClientUri = "http://localhost:5001",
                        AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                        ClientSecrets = {new Secret("the_secret".Sha256())},
                        AllowRememberConsent = true,
                        AllowOfflineAccess = true,
                        AllowAccessTokensViaBrowser = true,
                        AlwaysSendClientClaims = true,
                        AlwaysIncludeUserClaimsInIdToken = true,
                        RedirectUris = {
                            "http://localhost:5001/signin-oidc",
                        }, // after login
                        PostLogoutRedirectUris = { "http://localhost:5001/signout-callback-oidc"}, // after logout
                        AllowedScopes = new List<string>
                        {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            IdentityServerConstants.StandardScopes.Email,
                            IdentityServerConstants.StandardScopes.Phone,
                            IdentityServerConstants.StandardScopes.Address,
                            "roleIdentity",
                            "caseApi"
                        }
                    }
                };
        }
    }
}

