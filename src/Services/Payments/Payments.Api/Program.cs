using BuildingBlocks.Messaging;
using Payments.Application;
using Payments.Application.Consumers;
using Payments.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddMessageBus(builder.Configuration, "payments", x => x.AddConsumer<OrderPlacedConsumer>());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new() { Title = "SmartStore Payments API", Version = "v1" }));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
