using IdentityServer4.Models;

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
            }
        };
    }
}