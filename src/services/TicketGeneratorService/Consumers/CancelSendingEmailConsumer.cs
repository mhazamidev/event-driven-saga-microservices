using Events.SendEmailEvents;
using Events.TicketEvents;
using MassTransit;
using TicketGeneratorService.Services;

namespace TicketGeneratorService.Consumers;

public class CancelSendingEmailConsumer(ITicketInfoService ticketInfoService) : IConsumer<ICancelSendEmailEvent>
{
    public async Task Consume(ConsumeContext<ICancelSendEmailEvent> context)
    {
        var data = context.Message;

        if(data is not null)
        {
            var result = ticketInfoService.RemoveTicketInfo(data.TicketId.ToString());

            if(result is true)
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
