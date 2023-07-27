using Common;
using Discount.API.Extensions;
using Discount.API.Repositories;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(Constants.Authentication_Scheme_Bearer)
    .AddJwtBearer(Constants.Authentication_Scheme_Bearer, options =>
    {
        options.Authority = builder.Configuration.GetValue<string>("IdentityServe:Url");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
        options.RequireHttpsMetadata = false;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Constants.Client_Id_Policy, policy => policy.RequireClaim(Constants.Client_Id_Key, Constants.Shopping_Client_Id_Value));
});

var app = builder.Build();

app.MigrateDatabase<Program>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
