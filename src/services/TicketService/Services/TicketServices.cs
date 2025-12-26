using Microsoft.EntityFrameworkCore;
using TicketService.Models;

namespace TicketService.Services
{
    public class TicketServices : ITicketServices
    {
        private readonly AppDbContext _dbContext;

        public TicketServices(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Ticket> AddTicket(Ticket ticket)
        {
            if (ticket is not null)
            {
                _dbContext.Ticket.Add(ticket);
                await _dbContext.SaveChangesAsync();
            }
            return ticket;
        }

        public async Task<bool> ChangeTicketStatus(string TicketId, string status)
        {
            var ticketObj = await _dbContext.Ticket.FirstOrDefaultAsync(t => t.TicketId == TicketId);
            if (ticketObj is not null)
            {
                ticketObj.Status = status;
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteTicket(string TicketId)
        {
            var ticketObj = await _dbContext.Ticket.FirstOrDefaultAsync(t => t.TicketId == TicketId);
            if (ticketObj is not null)
            {
                _dbContext.Ticket.Remove(ticketObj);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
