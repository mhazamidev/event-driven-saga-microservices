using AutoMapper;
using Events.TicketEvents;
using TicketGeneratorService.Models;

namespace TicketGeneratorService.Mapping
{
    public class TicketInfoMapping : Profile
    {
        public TicketInfoMapping()
        {
            CreateMap<IGenerateTicketEvent, TicketInfo>();
        }
    }
}
