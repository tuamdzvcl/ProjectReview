using EventTick.Model.Models;
using projectDemo.DTO.Projection;
using projectDemo.DTO.Response;

namespace projectDemo.Repository.TickTypeRepository;

public interface ITypeTicketRepositorys
{
    Task<TicketType> GetTypeTickectByEventID(Guid EventID);
    Task<TicketType> CreateTicketType(TicketType ticketType);
    Task CreateRangeTicketTypes(IEnumerable<TicketType> ticketTypes);
    TicketType UpdateTicket(TicketType ticketType);
    string DeleteTicket(TicketType ticketType);
    TicketType? GetTicketTypebyId(int tickettype);
    Task<(EventProjection?, int statuss, string messager)> GetListTypeTickByEventID(Guid eventID);

}
