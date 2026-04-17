using EventTick.Model.Models;

namespace projectDemo.Repository.TickRepository
{
    public interface ITickRepository
    {
        Task<Ticket?> GetTicketById(int id);
        Task<Ticket> CreateTicket(Ticket ticket);
        int UpdateTicket(Ticket ticket);
        int DeleteTicket(Ticket ticket);
    }
}
