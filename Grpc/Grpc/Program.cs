using MediatR;
using RestaurantIntegrationGrpc.Command;
using RestaurantIntegrationGrpc.Domain;
using RestaurantIntegrationGrpc.Services;
using Sms.Test;

var builder = WebApplication.CreateBuilder(args);

// Регистрация MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddScoped<RestaurantIntegrationGrpc.Domain.IMenuRepository, MenuRepository>();

builder.Services.AddScoped<IRequestHandler<GetMenuQuery, Sms.Test.GetMenuResponse>, GetMenuQueryHandler>();


// Регистрация gRPC
builder.Services.AddGrpc();

builder.Services.AddGrpcReflection();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

// Включите gRPC-Web
app.UseGrpcWeb();

// Маппинг gRPC сервиса
app.MapGrpcService<SmsTestGrpcService>().EnableGrpcWeb();

app.Run();