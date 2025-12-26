using MassTransit;
using MessageBrokers;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TicketGeneratorService.Consumers;
using TicketGeneratorService.Models;
using TicketGeneratorService.Services;

var builder = WebApplication.CreateBuilder(args);

var dbHost = Environment.GetEnvironmentVariable("Db_Host");
var dbName = Environment.GetEnvironmentVariable("Db_Name");
var dbPasssword = Environment.GetEnvironmentVariable("Db_Password");
var dbPort = Environment.GetEnvironmentVariable("Db_Port");
var connectionString = $"server={dbHost};port={dbPort};database={dbName};user=root;password={dbPasssword}";

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());
builder.Services.AddScoped<ITicketInfoService, TicketInfoService>();

builder.Services.AddMassTransit(configure =>
{
    configure.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(RabbitMQConfig.RabbitMQHost);
        cfg.ReceiveEndpoint(RabbitMQQueues.GenerateTicketQueue, ep =>
        {
            ep.PrefetchCount = 10;
            ep.ConfigureConsumer<GenerateTicketConsumer>(ctx);
            ep.ConfigureConsumer<CancelSendingEmailConsumer>(ctx);
        });
    });

    configure.AddConsumer<GenerateTicketConsumer>();
    configure.AddConsumer<CancelSendingEmailConsumer>();
});

var app = builder.Build();


app.Run();

