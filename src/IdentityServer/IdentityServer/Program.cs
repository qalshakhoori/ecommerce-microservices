using IdentityServer;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var migrationAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
var connectionString = builder.Configuration.GetConnectionString("IdentityServerDB");

builder.Services.AddIdentityServer()
    .AddTestUsers(Config.TestUsers)
    .AddConfigurationStore(options =>
    {
        options.ConfigureDbContext = ctxBuilder => ctxBuilder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationAssembly));
    })
    .AddOperationalStore(options =>
    {
        options.ConfigureDbContext = ctxBuilder => ctxBuilder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationAssembly));
    })
    .AddDeveloperSigningCredential();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (builder.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseStaticFiles();
app.UseRouting();
app.UseIdentityServer();
app.UseAuthentication();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

using (var scope = app.Services.CreateScope())
{
    var grantDBContext = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();

    await grantDBContext.Database.MigrateAsync();

    var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

    await ContextSeed.SeedAsync(context);
}

app.Run();