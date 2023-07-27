using AspnetRunBasics.HttpHandlers;
using AspnetRunBasics.Services;
using Common;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        options.Authority = builder.Configuration.GetValue<string>("IdentityServer:Url");
        options.RequireHttpsMetadata = false;
        options.ClientId = Constants.Shopping_Client_Id_Value;
        options.ClientSecret = Constants.Shopping_Client_Secret;
        options.ResponseType = Constants.Response_Type;

        options.Scope.Add(Constants.Scope_OpenId);
        options.Scope.Add(Constants.Scope_Profile);
        options.Scope.Add(Constants.Scope_Address);
        options.Scope.Add(Constants.Scope_Email);
        options.Scope.Add(Constants.Scope_RoleValue);
        options.Scope.Add(Constants.Scope_Catalog_Api_Value);
        options.Scope.Add(Constants.Scope_Basket_Api_Value);
        options.Scope.Add(Constants.Scope_Discount_Api_Value);
        options.Scope.Add(Constants.Scope_Discount_Grpc_Value);
        options.Scope.Add(Constants.Scope_Order_Api_Value);

        options.ClaimActions.MapUniqueJsonKey(Constants.Scope_RoleValue, Constants.Scope_RoleValue);

        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;

        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            NameClaimType = JwtClaimTypes.GivenName,
            RoleClaimType = JwtClaimTypes.Role
        };
    });

builder.Services.AddTransient<AuthenticationDelegatingHandler>();


// add http client dependecy
builder.Services.AddHttpClient<ICatalogService, CatalogService>(c =>
    c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]))
    .AddHttpMessageHandler<AuthenticationDelegatingHandler>();
builder.Services.AddHttpClient<IBasketService, BasketService>(c =>
    c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]))
    .AddHttpMessageHandler<AuthenticationDelegatingHandler>();
builder.Services.AddHttpClient<IOrderService, OrderService>(c =>
    c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]))
    .AddHttpMessageHandler<AuthenticationDelegatingHandler>();

// add idp http client
builder.Services.AddHttpClient<IIdpService, IdpService>(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["IdentityServer:Url"]);
    c.DefaultRequestHeaders.Clear();
    c.DefaultRequestHeaders.Add(HeaderNames.Accept, Constants.Content_Type_Json);
});


builder.Services.AddHttpContextAccessor();

builder.Services.AddRazorPages();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
});

app.Run();