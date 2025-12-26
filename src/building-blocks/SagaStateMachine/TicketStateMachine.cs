using Events.SendEmailEvents;
using Events.TicketEvents;
using MassTransit;

namespace SagaStateMachine
{
    public class TicketStateMachine : MassTransitStateMachine<TicketStateData>
    {
        // 5 state
        public State AddTicket { get; set; }
        public State CancelTicket { get; set; }
        public State CancelSendEmail { get; set; }
        public State SendEmail { get; set; }
        public State Accepted { get; set; }

        // 5 event
        public Event<IAddTicketEvent> AddTicketEvent { get; set; }
        public Event<ICancelGenerateTicketEvent> CancelGenerateTicketEvent { get; set; }
        public Event<ICancelSendEmailEvent> CancelSendEmailEvent { get; private set; }
        public Event<ISendEmailEvent> SendEmailEvent { get; private set; }
        public Event<IAcceptTicketEvent> AcceptEvent { get; private set; }

        public TicketStateMachine()
        {
            InstanceState(s => s.CurrentState);
            Event(() => AddTicketEvent, a => a.CorrelateById(m => m.Message.TicketId));
            Event(() => CancelGenerateTicketEvent, a => a.CorrelateById(m => m.Message.TicketId));
            Event(() => CancelSendEmailEvent, a => a.CorrelateById(m => m.Message.TicketId));
            Event(() => SendEmailEvent, a => a.CorrelateById(m => m.Message.TicketId));
            Event(() => AcceptEvent, a => a.CorrelateById(m => m.Message.TicketId));

            Initially(
                When(AddTicketEvent).Then(context =>
                {
                    context.Saga.TicketId=context.Message.TicketId;
                    context.Saga.Status = "Pending";
                    context.Saga.Title = context.Message.Title;
                    context.Saga.Email = context.Message.Email;
                    context.Saga.TicketNumber = context.Message.TicketNumber;
                    context.Saga.Age = context.Message.Age;
                    context.Saga.Location = context.Message.Location;
                    context.Saga.TicketCreatedDate = DateTime.Now;
                }).TransitionTo(AddTicket).Publish(context=>new GenerateTicketEvent(context.Saga)));

            During(AddTicket,
                When(SendEmailEvent)
                .Then(context =>
                {
                    context.Saga.TicketId = context.Message.TicketId;
                    context.Saga.Status = "SendingEmail";
                    context.Saga.Title = context.Message.Title;
                    context.Saga.Email = context.Message.Email;
                    context.Saga.TicketNumber = context.Message.TicketNumber;
                    context.Saga.Age = context.Message.Age;
                    context.Saga.Location = context.Message.Location;
                }
                ).TransitionTo(SendEmail));

            During(AddTicket,
              When(CancelGenerateTicketEvent)
              .Then(context =>
              {
                  context.Saga.TicketId = context.Message.TicketId;
                  context.Saga.Status = "CancelGenerationNumber";
                  context.Saga.Title = context.Message.Title;
                  context.Saga.Email = context.Message.Email;
                  context.Saga.TicketNumber = context.Message.TicketNumber;
                  context.Saga.Age = context.Message.Age;
                  context.Saga.Location = context.Message.Location;
                  context.Saga.TicketCancelDate = DateTime.Now;
              }).TransitionTo(CancelTicket));

            During(SendEmail,
              When(CancelSendEmailEvent)
              .Then(context =>
              {
                  // These values could be different 
                  context.Saga.TicketId = context.Message.TicketId;
                  context.Saga.Status = "CancelSendEmail";
                  context.Saga.Title = context.Message.Title;
                  context.Saga.Email = context.Message.Email;
                  context.Saga.TicketNumber = context.Message.TicketNumber;
                  context.Saga.Age = context.Message.Age;
                  context.Saga.Location = context.Message.Location;
                  context.Saga.TicketCancelDate = DateTime.Now;
              }).TransitionTo(CancelSendEmail));

            // Final
            During(SendEmail,
                When(AcceptEvent)
                .Then(context =>
                {
                    // These values could be different 
                    context.Saga.TicketId = context.Message.TicketId;
                    context.Saga.Status = "Accepted";
                    context.Saga.Title = context.Message.Title;
                    context.Saga.Email = context.Message.Email;
                    context.Saga.TicketNumber = context.Message.TicketNumber;
                    context.Saga.Age = context.Message.Age;
                    context.Saga.Location = context.Message.Location;
                    context.Saga.TicketCancelDate = DateTime.Now;
                }).TransitionTo(Accepted));
        }
    }
}
