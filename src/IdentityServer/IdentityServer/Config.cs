using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Security.Claims;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<ApiScope> ApiScopes => new ApiScope[]
        {
            new ApiScope("catalog.api", "Catalog API")
        };

        public static IEnumerable<Client> Clients => new Client[]
        {
            new Client
            {
                ClientId = "shopping.client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },
                AllowedScopes =
                {
                    "catalog.api"
                }
            },
            new Client
            {
                ClientId = "shopping.web",
                ClientName = "Shopping Web App",
                AllowedGrantTypes= GrantTypes.Code,
                AllowRememberConsent = false,
                RedirectUris = new List<string>()
                {
                    "http://localhost:5007/signin-oidc", // this the client app port
                },
                PostLogoutRedirectUris = new List<string>()
                {
                    "http://localhost:5007/signout-callback-oidc"
                },
                ClientSecrets = new List<Secret>()
                {
                    new Secret("secret".Sha256())
                },
                AllowedScopes= new List<string>()
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile
                }
            }
        };

        public static IEnumerable<ApiResource> ApiResources => Array.Empty<ApiResource>();

        public static IEnumerable<IdentityResource> IdentityResources => new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };

        public static List<TestUser> TestUsers => new()
        {
            new TestUser
            {
                SubjectId = "4a5eadfd-3170-499c-a2aa-b81304422660",
                Username = "test",
                Password = "test",
                Claims = new List<Claim>
                {
                    new Claim(JwtClaimTypes.GivenName, "test given name"),
                    new Claim(JwtClaimTypes.FamilyName, "test family name")
                }
            }
        };
    }
}