using MassTransit;
using MessageBrokers;
using Microsoft.EntityFrameworkCore;
using SagaService.Models;
using SagaStateMachine;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


        builder.Services.AddMassTransit(configure =>
        {
            configure.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(RabbitMQConfig.RabbitMQHost);

                cfg.ReceiveEndpoint(RabbitMQQueues.SagaBusQueue, ep =>
                {
                    ep.PrefetchCount = 10;
                    ep.ConfigureSaga<TicketStateData>(ctx);
                });
            });

            configure.AddSagaStateMachine<TicketStateMachine, TicketStateData>()
                .EntityFrameworkRepository(r =>
                {
                    r.ConcurrencyMode = ConcurrencyMode.Pessimistic; // or use Optimistic, which requires RowVersion

                    r.ExistingDbContext<AppDbContext>();
                });
        });

        var app = builder.Build();


        app.Run();
    }
}