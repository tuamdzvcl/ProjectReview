using System.Data;
using Dapper;
using EventTick.Model.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using projectDemo.DTO.Projection;
using projectDemo.DTO.Response;
using projectDemo.Repository.BaseData;
using projectDemo.UnitOfWorks;

namespace projectDemo.Repository.TickTypeRepository
{
    public class TypeTicketRepositorys : RepositoryLinqBase<TicketType>, ITypeTicketRepositorys
    {
        private readonly RepositoryProcBase _proc;

        public TypeTicketRepositorys(IUnitOfWork uow)
            : base(uow)
        {
            _proc = new RepositoryProcBase(uow);
        }

        public async Task<TicketType> CreateTicketType(TicketType ticketType)
        {
            await _dbSet.AddAsync(ticketType);
            return ticketType;
        }

        public async Task CreateRangeTicketTypes(IEnumerable<TicketType> ticketTypes)
        {
            await _dbSet.AddRangeAsync(ticketTypes);
        }

        public async Task<List<TicketType>> GetByEventIdAsync(Guid eventId)
        {
            return await _dbSet
                .Where(x => x.EventID == eventId && x.IsDeleted == false)
                .ToListAsync();
        }

        public string DeleteTicket(TicketType ticketType)
        {
            _dbSet.Remove(ticketType);
            return "xóa thành công";
        }

        public async Task<(
            EventProjection?,
            int statuss,
            string messager
        )> GetListTypeTickByEventID(Guid eventID)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@eventID", eventID);
                param.Add("@status", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add(
                    "@messger",
                    dbType: DbType.String,
                    size: 250,
                    direction: ParameterDirection.Output
                );

                using var multi = await _uow.connection.QueryMultipleAsync(
                    "GetListTypeTicketbyEventID",
                    param,
                    commandType: CommandType.StoredProcedure
                );
                var events = await multi.ReadFirstOrDefaultAsync<EventProjection>();

                var typeticket = (await multi.ReadAsync<TicketType>()).ToList();

                if (events != null)
                {
                    events.listTypeTick = typeticket;
                }

                var status = param.Get<int>("@status");
                var message = param.Get<string>("@messger");

                return (events, status, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return (null, 404, ex.Message);
            }
        }

        public TicketType? GetTicketTypebyId(int tickettype)
        {
            return Find(x => x.Id == tickettype).FirstOrDefault();
        }

        public async Task<TicketType?> GetTypeTickectByEventID(Guid EventID)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@eventID", EventID);

                var result = await _uow.connection.QueryFirstOrDefaultAsync<TicketType>(
                    "GetTypeTickByEvent",
                    param,
                    commandType: CommandType.StoredProcedure
                );
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SyteamErorr: {ex.Message}");
                return null;
            }
        }

        public TicketType UpdateTicket(TicketType ticketType)
        {
            _dbSet.Update(ticketType);
            return ticketType;
        }
    }
}
