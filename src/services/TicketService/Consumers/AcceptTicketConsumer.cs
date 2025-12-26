using Events.TicketEvents;
using MassTransit;
using TicketService.Services;

namespace TicketService.Consumers;

public class AcceptTicketConsumer(ITicketServices ticketServices) : IConsumer<IAcceptTicketEvent>
{
    public async Task Consume(ConsumeContext<IAcceptTicketEvent> context)
    {
        var data = context.Message;

        if (data is not null)
        {
            await ticketServices.ChangeTicketStatus(data.TicketId.ToString(), "Accepted");
        }
    }
}
