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

        var dbHost = Environment.GetEnvironmentVariable("Db_Host");
        var dbName = Environment.GetEnvironmentVariable("Db_Name");
        var dbPasssword = Environment.GetEnvironmentVariable("Db_Password");
        var dbPort = Environment.GetEnvironmentVariable("Db_Port");
        var connectionString = $"server={dbHost};port={dbPort};database={dbName};user=root;password={dbPasssword}";

        builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));


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