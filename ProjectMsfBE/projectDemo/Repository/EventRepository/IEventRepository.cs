using EventTick.Model.Models;
using projectDemo.Common.PageRequest;
using projectDemo.DTO.Request;
using projectDemo.DTO.Respone;
using projectDemo.DTO.Response;
using projectDemo.DTO.UpdateRequest;

namespace projectDemo.Repository.Ipml
{
    public interface IEventRepository
    {
        Task<List<Event>> GetAllEvent();
        Task<Event> GetEventById(Guid EventID);
        Task CreateEvent(Event entity);
        Task<bool> UpdateEvent(Guid EventID, EventUpdateRequest request);
        void DeleteEvent(Event entity);
        Task<PageResponse<EventResponse>> GetPageEvent(int pageIndex, int pageSize, string key);
        Task<PageResponse<EventTypeTickResponses>> GetAllWithTicketTypesAsync(PageRequest request);
        Task<PageResponse<EventTypeTickResponses>> GetAllWithTicketTypesAsyncbyid(
            Guid id,
            PageRequest request
        );
        Task<EventTypeTickResponses?> GetEventDetailById(Guid eventId);
    }
}
