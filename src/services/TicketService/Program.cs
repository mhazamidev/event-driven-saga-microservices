using AutoMapper;
using Events.TicketEvents;
using MassTransit;
using MassTransit.MultiBus;
using MessageBrokers;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TicketService.Consumers;
using TicketService.DTO;
using TicketService.Models;
using TicketService.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());
builder.Services.AddScoped<ITicketServices, TicketServices>();


builder.Services.AddMassTransit(configure =>
{
    configure.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(RabbitMQConfig.RabbitMQHost);
        cfg.ReceiveEndpoint(RabbitMQQueues.AddTicketQueue, ep =>
        {
            ep.PrefetchCount = 10;
            ep.ConfigureConsumer<GenerateTicketCancelConsumer>(ctx);
            ep.ConfigureConsumer<AcceptTicketConsumer>(ctx);
        });
    });

    configure.AddConsumer<GenerateTicketCancelConsumer>();
    configure.AddConsumer<AcceptTicketConsumer>();
});


var app = builder.Build();


app.MapPost("/api/tikcet", async (ITicketServices ticketService, IMapper mapper, IPublishEndpoint endPoint, AddTicketDTO addTicketDTO) =>
{
    var mapModel = mapper.Map<Ticket>(addTicketDTO);
    mapModel.Status = "Pending";
    var res = await ticketService.AddTicket(mapModel);

    if (res is not null)
    {
        // map model to the DTO and pass the DTO object to the bus queue
        var mapResult = mapper.Map<ResponseTicketDTO>(res);

        // Send to the Bus
        await endPoint.Publish<IAddTicketEvent>(new
        {
            TicketId = Guid.Parse(mapResult.TicketId),
            mapResult.Title,
            mapResult.Email,
            mapResult.RequireDate,
            mapResult.Age,
            mapResult.Location,
        });

        return Results.Created();
    }

    return Results.BadRequest();
});

app.Run();

