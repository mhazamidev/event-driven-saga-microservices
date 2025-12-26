using MassTransit;
using MessageBrokers;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TicketGeneratorService.Consumers;
using TicketGeneratorService.Models;
using TicketGeneratorService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

