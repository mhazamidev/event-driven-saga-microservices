using Events.TicketEvents;
using MassTransit;
using TicketService.Services;

namespace TicketService.Consumers;

public class GenerateTicketCancelConsumer(ITicketServices ticketServices) : IConsumer<ICancelGenerateTicketEvent>
{
    public async Task Consume(ConsumeContext<ICancelGenerateTicketEvent> context)
    {
        var data = context.Message;

        if(data is not null)
        {
            await ticketServices.ChangeTicketStatus(data.TicketId.ToString(), "Rejected");
        }
    }
}
