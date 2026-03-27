using EventTick.Model.Models;
using projectDemo.DTO.Request;
using projectDemo.DTO.Respone;
using projectDemo.DTO.Response;
using projectDemo.DTO.UpdateRequest;

namespace projectDemo.Service.TicketTypeService
{
    public interface ITypeTicketService
    {
        Task<ApiResponse<TypeTickResponse>> GetTypeTickByEevntID(Guid EventID);
        Task<ApiResponse<EventTypeTickResponses>> GetListTypeTickByEventID(Guid eventID);
        Task<ApiResponse<TypeTickResponse>> CreateTypeTickect(TypeTicketRequest ticketType);
        Task<ApiResponse<string>> DeleteTypeTicket(int TypeTickectID);
        Task<ApiResponse<TypeTickResponse>> UpdateTypeTicket(int TypeTickectID,TypeTicketUpdate request);

    }
}
