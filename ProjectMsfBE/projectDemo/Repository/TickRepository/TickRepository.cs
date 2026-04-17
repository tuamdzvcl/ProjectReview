using EventTick.Model.Models;
using projectDemo.Repository.BaseData;
using projectDemo.UnitOfWorks;

namespace projectDemo.Repository.TickRepository
{
    public class TickRepository : RepositoryLinqBase<Ticket>, ITickRepository
    {
        public TickRepository(IUnitOfWork uow)
            : base(uow) { }

        public async Task<Ticket?> GetTicketById(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<Ticket> CreateTicket(Ticket ticket)
        {
            await AddAsync(ticket);
            return ticket;
        }

        public int UpdateTicket(Ticket ticket)
        {
            Update(ticket);
            return 1;
        }

        public int DeleteTicket(Ticket ticket)
        {
            Remove(ticket);
            return 1;
        }
    }
}
