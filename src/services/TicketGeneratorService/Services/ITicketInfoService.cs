using TicketGeneratorService.Models;

namespace TicketGeneratorService.Services
{
    public interface ITicketInfoService
    {
        Task<TicketInfo> AddTicketInfo(TicketInfo ticketInfo);
        bool RemoveTicketInfo(string TicketId);
    }
}
