using Common;
using EventBus.Messages.Common;
using MassTransit;
using Microsoft.IdentityModel.Tokens;
using Ordering.API.EventBusConsumer;
using Ordering.API.Extensions;
using Ordering.API.Mapping;
using Ordering.Application;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<BasketCheckoutConsumer>();

    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration.GetValue<string>("EventBusSettings:HostAddress"));

        cfg.ReceiveEndpoint(EventBusConstants.BASKET_CHECKOUT_QUEUE, c =>
        {
            c.ConfigureConsumer<BasketCheckoutConsumer>(ctx);
        });
    });
});

builder.Services.AddAutoMapper(typeof(OrderingProfile).Assembly);
builder.Services.AddScoped<BasketCheckoutConsumer>();

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

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var orderDBContext = scope.ServiceProvider.GetRequiredService<OrderContext>();

    app.MigrateDatabase<OrderContext>((context, services) =>
    {
        var logger = services.GetRequiredService<ILogger<OrderSeed>>();
        OrderSeed.SeedAsync(context, logger)
            .Wait();
    });
}

app.Run();