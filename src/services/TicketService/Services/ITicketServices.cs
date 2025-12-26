using TicketService.Models;

namespace TicketService.Services
{
    public interface ITicketServices
    {
        Task<Ticket> AddTicket(Ticket ticket);
        Task<bool> DeleteTicket(string TicketId);
        Task<bool> ChangeTicketStatus(string TicketId, string status);

        // Other methods like Update could be implemented 
    }
}
