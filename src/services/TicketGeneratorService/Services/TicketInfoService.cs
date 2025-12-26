using TicketGeneratorService.Common;
using TicketGeneratorService.Models;

namespace TicketGeneratorService.Services
{
    public class TicketInfoService(AppDbContext dbContext) : ITicketInfoService
    {
        public async Task<TicketInfo> AddTicketInfo(TicketInfo ticketInfo)
        {
            if (ticketInfo is not null)
            {
                ticketInfo.TicketNumber = StringGenerator.Generate();
                dbContext.TicketInfo.Add(ticketInfo);
                await dbContext.SaveChangesAsync();
            }
            return ticketInfo;
        }

        public bool RemoveTicketInfo(string TicketId)
        {
            var ticketInfoObj = dbContext.TicketInfo.FirstOrDefault(t => t.TicketId == TicketId);
            if (ticketInfoObj is not null)
            {
                dbContext.TicketInfo.Remove(ticketInfoObj);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
