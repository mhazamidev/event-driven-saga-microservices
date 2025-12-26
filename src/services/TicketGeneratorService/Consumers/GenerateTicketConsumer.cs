using AutoMapper;
using Events.SendEmailEvents;
using Events.TicketEvents;
using MassTransit;
using TicketGeneratorService.Models;
using TicketGeneratorService.Services;

namespace TicketGeneratorService.Consumers;

public class GenerateTicketConsumer(ITicketInfoService ticketInfoService,
    IMapper mapper) : IConsumer<IGenerateTicketEvent>
{
    public async Task Consume(ConsumeContext<IGenerateTicketEvent> context)
    {
        var data = context.Message;

        if (data is not null)
        {
            if (data.Age < 80)
            {
                var model = mapper.Map<TicketInfo>(data);

                var result = await ticketInfoService.AddTicketInfo(model);
                if(result is not null)
                {
                    await context.Publish<ISendEmailEvent>(new
                    {
                        data.TicketId,
                        data.Title,
                        data.Email,
                        data.RequireDate,
                        data.Age,
                        data.Location
                    });
                }
            }
            else
            {
                await context.Publish<ICancelGenerateTicketEvent>(new
                {
                    data.TicketId,
                    data.Title,
                    data.Email,
                    data.RequireDate,
                    data.Age,
                    data.Location
                });
            }
        }
    }
}
