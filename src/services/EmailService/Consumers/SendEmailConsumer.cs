using Events.SendEmailEvents;
using Events.TicketEvents;
using MassTransit;

namespace EmailService.Consumers;

public class SendEmailConsumer : IConsumer<ISendEmailEvent>
{
    public async Task Consume(ConsumeContext<ISendEmailEvent> context)
    {
        var data = context.Message;

        if(data is not null)
        {
            if (data.Location.Equals("London",StringComparison.OrdinalIgnoreCase))
            {
                await context.Publish<ICancelSendEmailEvent>(new
                {
                    data.TicketId,
                    data.Title,
                    data.Email,
                    data.RequireDate,
                    data.Age,
                    data.Location
                });
            }
            else
            {
                //send email
                await context.Publish<IAcceptTicketEvent>(new
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
