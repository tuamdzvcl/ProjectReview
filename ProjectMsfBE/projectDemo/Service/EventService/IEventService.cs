using projectDemo.Common.PageRequest;
using projectDemo.DTO.Request;
using projectDemo.DTO.Respone;
using projectDemo.DTO.Response;
using projectDemo.DTO.UpdateRequest;
using System.Management;

namespace projectDemo.Service.EventService
{
    public interface IEventService
    {
        Task<List<EventResponse>> GetEventAll();

        Task<ApiResponse<EventTypeTickResponses>> GetEventById(Guid EventID);
        Task<ApiResponse<string>> UpdateEvent(Guid EventID, EventUpdateRequest resquest);
        Task<ApiResponse<string>> DeleteEvent(Guid EventID);
        Task<ApiResponse<EventResponse>> CreateEvent(EventRequest resquest,Guid Userid);
        Task<ApiResponse<CreateEventWithTicketTypesResponse>> CreateEventWithTicketTypes(CreateEventWithTicketTypesRequest request, Guid userId);
        bool checkVadidate(EventRequest resquest);
        Task<PageResponse<EventResponse>> GetListEventPage(int pageSize, int pageIndex, string keyWord);

        Task<PageResponse<EventTypeTickResponses>> GetPageWithTicketTypes(PageRequest query);
    }
}
