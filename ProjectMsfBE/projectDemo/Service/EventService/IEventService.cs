using System.Management;
using projectDemo.Common.PageRequest;
using projectDemo.DTO.Request;
using projectDemo.DTO.Respone;
using projectDemo.DTO.Response;
using projectDemo.DTO.UpdateRequest;

namespace projectDemo.Service.EventService
{
    public interface IEventService
    {
        Task<List<EventResponse>> GetEventAll();

        Task<ApiResponse<EventTypeTickResponses>> GetEventById(Guid EventID);
        Task<ApiResponse<string>> UpdateEvent(Guid EventID, EventUpdateRequest resquest);
        Task<ApiResponse<string>> UpdateEventStatus(Guid eventId, EventStatusUpdateRequest request);
        Task<ApiResponse<string>> DeleteEvent(Guid EventID);
        Task<ApiResponse<CreateEventWithTicketTypesResponse>> CreateEventWithTicketTypes(
            CreateEventWithTicketTypesRequest request,
            Guid userId
        );
        Task<ApiResponse<string>> DuplicateEvent(Guid eventId);
        bool checkVadidate(EventRequest resquest);

        Task<PageResponse<EventResponse>> GetListEventPage(
            int pageSize,
            int pageIndex,
            string keyWord
        );
        Task<PageResponse<EventTypeTickResponses>> GetPageWithTicketTypes(PageRequest query, bool isAdmin = false);
        Task<PageResponse<EventTypeTickResponses>> GetPageWithTicketTypesbyId(
            Guid id,
            PageRequest query
        );
        //Task<ApiResponse<EventResponse>> CreateEvent(EventRequest resquest,Guid Userid);
    }
}
