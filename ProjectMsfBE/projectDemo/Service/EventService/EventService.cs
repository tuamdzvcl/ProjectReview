
using projectDemo.Entity.Models;
using projectDemo.DTO.Respone;
using projectDemo.DTO.Response;
using projectDemo.Service.Auth;
using projectDemo.Repository.Ipml;
using AutoMapper;
using projectDemo.DTO.Request;
using EventTick.Model.Models;
using EventTick.Model.Enum;
using projectDemo.DTO.UpdateRequest;
using projectDemo.Service.ImageService;
using static System.Net.Mime.MediaTypeNames;
using Azure.Core;
using static Dapper.SqlMapper;
using projectDemo.UnitOfWorks;
using projectDemo.Repository.CatetoryRepository;
using System.Management;
using projectDemo.Common.PageRequest;
using Microsoft.Extensions.Logging;

namespace projectDemo.Service.EventService
{

    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IImageService _imageService;
        private readonly IUserReposiotry _userReposiotry;
        private readonly IUnitOfWork _uow;
        private readonly ICatetoryReposioty _catrtoeyRepository;
        private readonly IMapper _mapper;

        public EventService(ICatetoryReposioty catetoryReposioty,IUnitOfWork uow,IImageService imageService,IEventRepository eventRepository, IMapper mapper, IUserReposiotry userReposiotry)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
            _userReposiotry = userReposiotry;
            _imageService = imageService;
            _uow = uow;
            _catrtoeyRepository = catetoryReposioty;
        }
        //check date
        public bool checkVadidate(EventRequest request)
        {
            DateTime now = DateTime.Now;

            if (request.StartDate <= now)
            {
                return false;
            }
            if (request.EndDate <= request.StartDate)
                return false;
            if (request.SaleStartDate >= request.SaleEndDate)
                return false;
            if (request.SaleEndDate >= request.StartDate)
                return false;
            if (request.EndDate - request.StartDate > TimeSpan.FromDays(3))
                return false;
            return true;
        }


        //lấy userName
        public async Task<List<string>> rederNameByUserID(Guid UserID)
        {
            return await _userReposiotry.GetRoleByUser(UserID);
        }
        //tạo event -> chưa tạo typetick
        public async Task<ApiResponse<EventResponse>> CreateEvent(EventRequest resquest,Guid Userid)
        {
            try
            {
                var check = checkVadidate(resquest);
                

                if (!check)
                {
                    return ApiResponse<EventResponse>.FailResponse(Entity.Enum.EnumStatusCode.DATE, "Kiêm tra lại ngày và giờ");
                }
                var userid = await _userReposiotry.GetUserByid(Userid);
                if (userid == null)
                {
                    return ApiResponse<EventResponse>.FailResponse(Entity.Enum.EnumStatusCode.USERNOTFOUND, "Không tìm thấy user ");

                }
                var catetory =await _catrtoeyRepository.GetByName(resquest.CatetoryName.ToUpper());
                if (catetory == null)
                {
                    return ApiResponse<EventResponse>.FailResponse(Entity.Enum.EnumStatusCode.NOT_FOUND, "Không tìm thấy thể loại ");
                }

                var image = await _imageService.UploadAsync(resquest.PosterUrl);

                Event events = new Event
                {
                    Id = Guid.NewGuid(),
                    UserID = Userid,
                    Title = resquest.Title,
                    Status = EnumStatusEvent.DRAFT,
                    PosterUrl = image,
                    StartDate = resquest.StartDate,
                    EndDate = resquest.EndDate,
                    SaleStartDate = resquest.SaleStartDate,
                    SaleEndDate = resquest.SaleEndDate,
                    Description = resquest.Description,
                    Location = resquest.Location,
                    CreatedDate = DateTime.Now,
                    CatetoryID=catetory.Id,
                    IsDeleted= false

                };
                await _eventRepository.CreateEvent(events);
                await _uow.SaveChangesAsync();

                EventResponse response = new EventResponse
                {
                    UserID = events.UserID,
                    Status = events.Status.ToString(),
                    PosterUrl = events.PosterUrl,
                    StartDate = events.StartDate,
                    EndDate = events.EndDate,
                    SaleStartDate = events.SaleStartDate,
                    SaleEndDate = events.SaleEndDate,
                    Description = events.Description,
                    Location = events.Location,
                    EventID = events.Id,
                    Title = events.Title
                };
                return ApiResponse<EventResponse>.SuccessResponse(Entity.Enum.EnumStatusCode.SUCCESS, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"System Error: {ex.Message}");

                return ApiResponse<EventResponse>.FailResponse(Entity.Enum.EnumStatusCode.SERVER, "lỗi", ex.Message);
            }


        }
        // xóa mềm
        public async Task<ApiResponse<string>> DeleteEvent(Guid EventID)
        {
            try
            {
                Event events = await _eventRepository.GetEventById(EventID)
                 ?? throw new DllNotFoundException();
                events.Status = EnumStatusEvent.CANNEL;
                await _uow.SaveChangesAsync();
                return ApiResponse<string>.SuccessResponse(Entity.Enum.EnumStatusCode.SUCCESS, "Xóa thành công");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"System Error: {ex.Message}");

                return ApiResponse<string>.FailResponse(Entity.Enum.EnumStatusCode.SERVER, "lỗi", ex.Message);
            }
        }
        // lấy tất cả các event
        public async Task<List<EventResponse>> GetEventAll()
        {
            List<Event> listEvents = await _eventRepository.GetAllEvent();

            var result = listEvents.Select(e => new EventResponse
            {
                EventID = e.Id,
                Description = e.Description,
                Location = e.Location,
                EndDate = e.EndDate,
                SaleStartDate = e.SaleStartDate,
                SaleEndDate = e.SaleEndDate,
                StartDate = e.StartDate,
                PosterUrl = e.PosterUrl,
                Status = e.Status.ToString(),
                Title = e.Title,
                UserID = e.UserID
            }).ToList();
            return result;
        }
        //lấy event theo id
        public async Task<ApiResponse<EventTypeTickResponses>> GetEventById(Guid EventID)
        {
            try
            {
                var response = await _eventRepository.GetEventDetailById(EventID);
                return ApiResponse<EventTypeTickResponses>.SuccessResponse(Entity.Enum.EnumStatusCode.SUCCESS, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"System Error: {ex.Message}");

                return ApiResponse<EventTypeTickResponses>.FailResponse(Entity.Enum.EnumStatusCode.SERVER, "lỗi", ex.Message);
            }

        }
        // lấy tất cả các event phân trang
        public async Task<PageResponse<EventResponse>> GetListEventPage(int pageSize, int pageIndex, string keyWord)
        {
            try
            {
                if (pageIndex < 1)
                    pageIndex = 1;
                if (pageSize <= 0)
                    pageSize = 10;

                var response = await _eventRepository.GetPageEvent(pageIndex, pageSize, keyWord);
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine("lỗi ở đây này :", ex.Message);
                throw new Exception();
            }
        }
        //update event done
        public async Task<ApiResponse<string>> UpdateEvent(Guid EventID, EventUpdateRequest resquest)
        {
            try
            {
                var events = await _eventRepository.GetEventById(EventID);
                if (events == null)
                    return ApiResponse<string>.FailResponse(Entity.Enum.EnumStatusCode.EVENTNOTFOUD, "even không tồn tại ");
                if (resquest.PosterUrl != null)
                {
                    _imageService.Delete(events.PosterUrl);
                    var newImageUrl = await _imageService.UploadAsync(resquest.PosterUrl);

                    events.PosterUrl = newImageUrl;
                }

                var catetory = await _catrtoeyRepository.GetByName(resquest.CatetoryName.ToLower());
                if(catetory == null)
                {
                    return ApiResponse<string>.FailResponse(Entity.Enum.EnumStatusCode.EVENTNOTFOUD, "catetory không tồn tại không tồn tại ");

                }

                events.Title = resquest.Title ?? events.Title;
                events.Status = resquest.Status ?? events.Status;
                events.EndDate = resquest.EndDate ?? events.EndDate;
                events.StartDate = resquest.StartDate ?? events.StartDate;
                events.SaleStartDate = resquest.SaleStartDate ?? events.SaleStartDate;
                events.SaleEndDate = resquest.SaleEndDate ?? events.SaleEndDate;
                events.Description = resquest.Description ?? events.Description;
                events.Location = resquest.Location ?? events.Location;
                events.CatetoryID = catetory.Id;
                events.UpdatedDate = DateTime.Now;

                await _uow.SaveChangesAsync();
                return ApiResponse<string>.SuccessResponse(Entity.Enum.EnumStatusCode.SUCCESS, "Update Thành công");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"System Error: {ex.Message}");
                return ApiResponse<string>.FailResponse(Entity.Enum.EnumStatusCode.SERVER, "lỗi", ex.Message);
            }
        }

        public async Task<PageResponse<EventTypeTickResponses>> GetPageWithTicketTypes(PageRequest query)
        {
            var key = query.key;
            var index = query.PageIndex;
            var size = query.PageSize;

            if(index == 0) index = 1;
            if(size == 0) size = 10;

           return await _eventRepository.GetAllWithTicketTypesAsync(query);

        }
    }
}
