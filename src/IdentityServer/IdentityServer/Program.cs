using IdentityServer;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var migrationAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
var connectionString = builder.Configuration.GetConnectionString("IdentityServerDB");

builder.Services.AddIdentityServer()
    .AddTestUsers(Config.Users)
    .AddConfigurationStore(options =>
    {
        options.ConfigureDbContext = ctxBuilder => ctxBuilder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationAssembly));
    })
    .AddOperationalStore(options =>
    {
        options.ConfigureDbContext = ctxBuilder => ctxBuilder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationAssembly));
    })
    .AddDeveloperSigningCredential();

builder.Services
    //.AddAuthentication(options =>
    //{
    //    options.DefaultScheme = "cookies";
    //    options.DefaultChallengeScheme = "oidc";
    //})
    //.AddCookie("cookies", options =>
    //{
    //    options.Cookie.Name = "appcookie";
    //    options.Cookie.SameSite = SameSiteMode.Strict;
    //    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    //})
    //.AddOpenIdConnect("oidc", options =>
    //{
    //    options.NonceCookie.SecurePolicy = CookieSecurePolicy.Always;
    //    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
    //});
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
           .AddCookie(options =>
           {
               // add an instance of the patched manager to the options:
               options.CookieManager = new ChunkingCookieManager();

               options.Cookie.HttpOnly = true;
               options.Cookie.SameSite = SameSiteMode.None;
               options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
           });

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (builder.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseStaticFiles();
app.UseRouting();
app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();

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