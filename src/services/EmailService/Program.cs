using EmailService.Consumers;
using MassTransit;
using MessageBrokers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(configure =>
{
    configure.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(RabbitMQConfig.RabbitMQHost);

        cfg.ReceiveEndpoint(RabbitMQQueues.SendEmailQueue, ep =>
        {
            ep.PrefetchCount = 10;
            ep.ConfigureConsumer<SendEmailConsumer>(ctx);
        });
    });

    configure.AddConsumer<SendEmailConsumer>();
});

var app = builder.Build();

app.Run();

