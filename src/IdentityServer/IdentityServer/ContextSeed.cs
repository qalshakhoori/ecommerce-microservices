using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer
{
    public class ContextSeed
    {
        public static async Task SeedAsync(ConfigurationDbContext context)
        {
            await context.Database.MigrateAsync();

            if (!context.Clients.Any())
            {
                foreach (var client in Config.Clients)
                    context.Clients.Add(client.ToEntity());

                await context.SaveChangesAsync();
            }

            if(!context.IdentityResources.Any())
            {
                foreach(var resource in Config.IdentityResources)
                    context.IdentityResources.Add(resource.ToEntity());

                await context.SaveChangesAsync();
            }

            if(!context.ApiResources.Any())
            {
                foreach (var resource in Config.ApiResources)
                    context.ApiResources.Add(resource.ToEntity());

                await context.SaveChangesAsync();
            }

            if (!context.ApiScopes.Any())
            {
                foreach (var scope in Config.ApiScopes)
                    context.ApiScopes.Add(scope.ToEntity());

                await context.SaveChangesAsync();
            }
        }
    }
}