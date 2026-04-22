using System.Linq;
using AutoMapper;
using EventTick.Model.Enum;
using EventTick.Model.Models;
using Microsoft.EntityFrameworkCore;
using projectDemo.Common.PageRequest;
using projectDemo.DTO.Request;
using projectDemo.DTO.Respone;
using projectDemo.DTO.Response;
using projectDemo.DTO.UpdateRequest;
using projectDemo.Entity.Enum;
using projectDemo.Entity.Models;
using projectDemo.Repository.CatetoryRepository;
using projectDemo.Repository.Ipml;
using projectDemo.Repository.TickTypeRepository;
using projectDemo.Repository.UpgradeRepository;
using projectDemo.Repository.UserUpgradeRepository;
using projectDemo.Service.Auth;
using projectDemo.Service.ImageService;
using projectDemo.UnitOfWorks;

namespace projectDemo.Service.EventService
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IImageService _imageService;
        private readonly IUserReposiotry _userReposiotry;
        private readonly IUnitOfWork _uow;
        private readonly ICatetoryReposioty _catrtoeyRepository;
        private readonly ITypeTicketRepositorys _typeTicketRepository;
        private readonly IAuthRepository _authRepository;
        private readonly IUpgradeRepository _upgradeRepository;
        private readonly IUserUpgradeRepository _userUpgradeRepository;
        private readonly IMapper _mapper;

        public EventService(
            ICatetoryReposioty catetoryReposioty,
            IAuthRepository auth,
            IUnitOfWork uow,
            IImageService imageService,
            IEventRepository eventRepository,
            IMapper mapper,
            IUserReposiotry userReposiotry,
            ITypeTicketRepositorys typeTicketRepository,
            IUpgradeRepository upgradeRepository,
            IUserUpgradeRepository userUpgradeRepository
        )
        {
            _eventRepository = eventRepository;
            _authRepository = auth;
            _mapper = mapper;
            _userReposiotry = userReposiotry;
            _imageService = imageService;
            _uow = uow;
            _catrtoeyRepository = catetoryReposioty;
            _typeTicketRepository = typeTicketRepository;
            _upgradeRepository = upgradeRepository;
            _userUpgradeRepository = userUpgradeRepository;
        }

        private static TimeZoneInfo GetVietnamTimeZone()
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
            }
        }

        private static DateTime GetVietnamNow()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, GetVietnamTimeZone());
        }

        private static bool IsEventEnded(Event events, DateTime now)
        {
            return events.EndDate.HasValue && events.EndDate.Value <= now;
        }

        private async Task SyncEndedEventsAsync()
        {
            var now = GetVietnamNow();
            var expiredEvents = await _uow
                .context.Set<Event>()
                .Where(e =>
                    e.IsDeleted == false
                    && e.Status != EnumStatusEvent.CANNEL
                    && e.Status != EnumStatusEvent.ENDED
                    && e.EndDate.HasValue
                    && e.EndDate.Value <= now
                )
                .ToListAsync();

            if (!expiredEvents.Any())
                return;

            foreach (var item in expiredEvents)
            {
                item.Status = EnumStatusEvent.ENDED;
                item.UpdatedDate = now;
            }

            await _uow.SaveChangesAsync();
        }

        private async Task EnsureEventEndedStatusAsync(Event events)
        {
            var now = GetVietnamNow();
            if (!IsEventEnded(events, now))
                return;

            if (events.Status == EnumStatusEvent.CANNEL || events.Status == EnumStatusEvent.ENDED)
                return;

            events.Status = EnumStatusEvent.ENDED;
            events.UpdatedDate = now;
            await _uow.SaveChangesAsync();
        }

        //check date
        public bool checkVadidate(EventRequest request)
        {
            // Lấy thời gian hiện tại theo múi giờ Việt Nam (GMT+7)
            var now = GetVietnamNow();

            var hasAnyDate =
                request.StartDate.HasValue
                || request.EndDate.HasValue
                || request.SaleStartDate.HasValue
                || request.SaleEndDate.HasValue;

            if (!hasAnyDate)
                return true;

            if (
                !request.StartDate.HasValue
                || !request.EndDate.HasValue
                || !request.SaleStartDate.HasValue
                || !request.SaleEndDate.HasValue
            )
            {
                return false;
            }

            if (request.StartDate.Value <= now)
            {
                return false;
            }
            if (request.EndDate.Value <= request.StartDate.Value)
                return false;
            if (request.SaleStartDate.Value >= request.SaleEndDate.Value)
                return false;
            if (request.SaleEndDate.Value > request.StartDate.Value)
                return false;
            return true;
        }

        private static bool IsEventNotFound(Event events)
        {
            return events == null || events.Id == Guid.Empty;
        }

        // gắn entity->dto
        private static EventRequest BuildEventValidationRequest(
            Event existingEvent,
            EventUpdateRequest request
        )
        {
            return new EventRequest
            {
                Title = request.Title ?? existingEvent.Title,
                Description = request.Description ?? existingEvent.Description,
                Location = request.Location ?? existingEvent.Location,
                StartDate = request.StartDate ?? existingEvent.StartDate,
                EndDate = request.EndDate ?? existingEvent.EndDate,
                SaleStartDate = request.SaleStartDate ?? existingEvent.SaleStartDate,
                SaleEndDate = request.SaleEndDate ?? existingEvent.SaleEndDate,
                CatetoryName = request.CatetoryName ?? existingEvent.Catetory?.Name ?? string.Empty,
                PosterUrl = request.PosterUrl,
            };
        }

        private static bool HasInvalidTicketTypes(
            IEnumerable<CreateEventTicketTypeItemRequest> ticketTypes
        )
        {
            return ticketTypes.Any(ticket => ticket.TotalQuantity <= 0 || ticket.Price <= 0);
        }

        private static bool IsTicketTypeUpdateInvalid(UpdateEventTicketTypeItemRequest ticket)
        {
            if (ticket.Name == null)
                return true;

            if (!ticket.TotalQuantity.HasValue)
                return true;

            if (!ticket.Price.HasValue)
                return true;

            if (ticket.Status == null)
                return true;

            int totalQuantity = ticket.TotalQuantity.Value;
            decimal price = ticket.Price.Value;

            if (totalQuantity <= 0)
                return true;

            if (price <= 0)
                return true;

            return false;
        }

        private static bool HasInvalidTicketTypeUpdates(
            IEnumerable<UpdateEventTicketTypeItemRequest> ticketTypes
        )
        {
            foreach (var ticket in ticketTypes)
            {
                if (IsTicketTypeUpdateInvalid(ticket))
                    return true;
            }

            return false;
        }

        //lấy userName
        public async Task<List<string>> rederNameByUserID(Guid UserID)
        {
            return await _userReposiotry.GetRoleByUser(UserID);
        }

        public async Task<
            ApiResponse<CreateEventWithTicketTypesResponse>
        > CreateEventWithTicketTypes(CreateEventWithTicketTypesRequest request, Guid userId)
        {
            string? imageUrl = null;
            var transactionStarted = false;

            try
            {
               

                // 2. Các kiểm tra dữ liệu hiện tại
                if (!checkVadidate(request))
                {
                    return ApiResponse<CreateEventWithTicketTypesResponse>.FailResponse(
                        EnumStatusCode.DATE,
                        "Kiểm tra lại ngày giờ của event"
                    );
                }

                if (request.TicketTypes == null || !request.TicketTypes.Any())
                {
                    return ApiResponse<CreateEventWithTicketTypesResponse>.FailResponse(
                        EnumStatusCode.BAD_REQUEST,
                        "Event phải có ít nhất 1 loại vé"
                    );
                }

                if (HasInvalidTicketTypes(request.TicketTypes))
                {
                    return ApiResponse<CreateEventWithTicketTypesResponse>.FailResponse(
                        EnumStatusCode.BAD_REQUEST,
                        "Thông tin loại vé không hợp lệ"
                    );
                }

                if (request.PosterUrl == null)
                {
                    return ApiResponse<CreateEventWithTicketTypesResponse>.FailResponse(
                        EnumStatusCode.BAD_REQUEST,
                        "PosterUrl không được để trống"
                    );
                }

                var user = await _userReposiotry.GetUserByid(userId);
                if (user == null)
                {
                    return ApiResponse<CreateEventWithTicketTypesResponse>.FailResponse(
                        EnumStatusCode.USERNOTFOUND,
                        "Không tìm thấy user"
                    );
                }

                var catetory = await _catrtoeyRepository.GetByName(request.CatetoryName.ToUpper());
                if (catetory == null)
                {
                    catetory = new Catetory
                    {
                        Id = Guid.NewGuid(),
                        Name = request.CatetoryName
                    };
                    await _catrtoeyRepository.Create(catetory);
                }

                await _uow.BeginTransactionAsync();
                transactionStarted = true;

                imageUrl = await _imageService.UploadAsync(request.PosterUrl);

                var eventEntity = new Event
                {
                    Id = Guid.NewGuid(),
                    UserID = userId,
                    Title = request.Title,
                    Status = EnumStatusEvent.DRAFT,
                    PosterUrl = imageUrl,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    SaleStartDate = request.SaleStartDate,
                    SaleEndDate = request.SaleEndDate,
                    Description = request.Description,
                    Location = request.Location,
                    CreatedDate = DateTime.Now,
                    CatetoryID = catetory.Id,
                    IsDeleted = false,
                };

                var ticketTypeEntities = request
                    .TicketTypes.Select(ticket => new TicketType
                    {
                        Name = ticket.Name,
                        Price = ticket.Price,
                        TotalQuantity = ticket.TotalQuantity,
                        Status = ticket.Status,
                        EventID = eventEntity.Id,
                        IsDeleted = false,
                        CreatedDate = DateTime.Now,
                    })
                    .ToList();

                await _eventRepository.CreateEvent(eventEntity);
                await _typeTicketRepository.CreateRangeTicketTypes(ticketTypeEntities);

             

                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();

                var response = new CreateEventWithTicketTypesResponse
                {
                    EventID = eventEntity.Id,
                    UserID = eventEntity.UserID,
                    Title = eventEntity.Title,
                    Description = eventEntity.Description,
                    Location = eventEntity.Location,
                    StartDate = eventEntity.StartDate,
                    EndDate = eventEntity.EndDate,
                    SaleStartDate = eventEntity.SaleStartDate,
                    SaleEndDate = eventEntity.SaleEndDate,
                    PosterUrl = eventEntity.PosterUrl,
                    Status = eventEntity.Status.ToString(),
                    CatetoryName = catetory.Name,
                    TicketTypes = ticketTypeEntities
                        .Select(ticket => new TypeTickResponse
                        {
                            Id = ticket.Id,
                            Name = ticket.Name.ToString(),
                            Price = ticket.Price,
                            TotalQuantity = ticket.TotalQuantity,
                            SoldQuantity = ticket.SoldQuantity,
                            Status = ticket.Status.ToString(),
                        })
                        .ToList(),
                };

                return ApiResponse<CreateEventWithTicketTypesResponse>.SuccessResponse(
                    EnumStatusCode.SUCCESS,
                    response
                );
            }
            catch (Exception ex)
            {
                if (transactionStarted)
                {
                    await _uow.RollbackAsync();
                }
                Console.WriteLine($"System Error: {ex.Message}");

                if (!string.IsNullOrWhiteSpace(imageUrl))
                {
                    _imageService.Delete(imageUrl);
                }

                return ApiResponse<CreateEventWithTicketTypesResponse>.FailResponse(
                    EnumStatusCode.SERVER,
                    "Lỗi khi tạo event",
                    ex.Message
                );
            }
        }

        // xóa mềm
        public async Task<ApiResponse<string>> DeleteEvent(Guid EventID,bool isAdmin =false)
        {
            try
            {
                await SyncEndedEventsAsync();

                

                Event events = await _eventRepository.GetEventById(EventID);

                if (events == null)
                {
                    return ApiResponse<string>.FailResponse(
                        Entity.Enum.EnumStatusCode.EVENTNOTFOUD,
                        "Không tìm thấy event"
                    );
                }
                if (!isAdmin)
                {
                    return ApiResponse<string>.FailResponse(
                                           Entity.Enum.EnumStatusCode.BAD_REQUEST,
                                           "Có phải admin không mà đòi xóa"
                                       );
                }
                await EnsureEventEndedStatusAsync(events);

                if (events.Status == EnumStatusEvent.PUBLIC)
                {
                    return ApiResponse<string>.FailResponse(
                        Entity.Enum.EnumStatusCode.UNAUTHORIZED,
                        "Sự kiện đã được công bố không thu hồi được"
                    );
                }
                events.Status = EnumStatusEvent.CANNEL;
                events.IsDeleted = true;
                await _uow.SaveChangesAsync();
                return ApiResponse<string>.SuccessResponse(
                    Entity.Enum.EnumStatusCode.SUCCESS,
                    "Xóa thành công"
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"System Error: {ex.Message}");

                return ApiResponse<string>.FailResponse(
                    Entity.Enum.EnumStatusCode.SERVER,
                    "lỗi",
                    ex.Message
                );
            }
        }

        // lấy tất cả các event
        public async Task<List<EventResponse>> GetEventAll()
        {
            await SyncEndedEventsAsync();
            List<Event> listEvents = await _eventRepository.GetAllEvent();

            var result = listEvents
                .Select(e => new EventResponse
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
                    UserID = e.UserID,
                })
                .ToList();
            return result;
        }

        //lấy event theo id
        public async Task<ApiResponse<EventTypeTickResponses>> GetEventById(Guid EventID)
        {
            try
            {
                await SyncEndedEventsAsync();
                var response = await _eventRepository.GetEventDetailById(EventID);
                return ApiResponse<EventTypeTickResponses>.SuccessResponse(
                    Entity.Enum.EnumStatusCode.SUCCESS,
                    response
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"System Error: {ex.Message}");

                return ApiResponse<EventTypeTickResponses>.FailResponse(
                    Entity.Enum.EnumStatusCode.SERVER,
                    "lỗi",
                    ex.Message
                );
            }
        }

        // lấy tất cả các event phân trang
        public async Task<PageResponse<EventResponse>> GetListEventPage(
            int pageSize,
            int pageIndex,
            string keyWord
        )
        {
            try
            {
                await SyncEndedEventsAsync();
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

        // update event done
        public async Task<ApiResponse<string>> UpdateEvent(
            Guid EventID,
            EventUpdateRequest resquest
        )
        {
            string? oldImageUrl = null;
            string? newImageUrl = null;
            var transactionStarted = false;
            try
            {
                await SyncEndedEventsAsync();
                var events = await _eventRepository.GetEventById(EventID);
                if (IsEventNotFound(events))
                    return ApiResponse<string>.FailResponse(
                        Entity.Enum.EnumStatusCode.EVENTNOTFOUD,
                        "even không tồn tại "
                    );

                await EnsureEventEndedStatusAsync(events);
                if (events.Status == EnumStatusEvent.ENDED)
                {
                    return ApiResponse<string>.FailResponse(
                        Entity.Enum.EnumStatusCode.BAD_REQUEST,
                        "Sự kiện đã kết thúc, không thể chỉnh sửa"
                    );
                }

                var validationRequest = BuildEventValidationRequest(events, resquest);
                if (!checkVadidate(validationRequest))
                {
                    return ApiResponse<string>.FailResponse(
                        Entity.Enum.EnumStatusCode.DATE,
                        "Kiểm tra lại ngày giờ của event"
                    );
                }

                if (
                    resquest.TicketTypes != null
                    && resquest.TicketTypes.Any()
                    && HasInvalidTicketTypeUpdates(resquest.TicketTypes)
                )
                {
                    return ApiResponse<string>.FailResponse(
                        Entity.Enum.EnumStatusCode.BAD_REQUEST,
                        "Thông tin loại vé không hợp lệ"
                    );
                }

                await _uow.BeginTransactionAsync();
                transactionStarted = true;

                if (!string.IsNullOrWhiteSpace(resquest.CatetoryName))
                {
                    var catetory = await _catrtoeyRepository.GetByName(
                        resquest.CatetoryName.ToUpper()
                    );
                    if (catetory == null)
                    {
                        catetory = new Catetory
                        {
                            Id = Guid.NewGuid(),
                            Name = resquest.CatetoryName
                        };
                        await _catrtoeyRepository.Create(catetory);
                    }

                    events.CatetoryID = catetory.Id;
                }

                if (resquest.PosterUrl != null)
                {
                    oldImageUrl = events.PosterUrl;
                    newImageUrl = await _imageService.UploadAsync(resquest.PosterUrl);
                    events.PosterUrl = newImageUrl;
                }

                events.Title = resquest.Title ?? events.Title;
                events.Status = resquest.Status ?? events.Status;
                events.EndDate = resquest.EndDate ?? events.EndDate;
                events.StartDate = resquest.StartDate ?? events.StartDate;
                events.SaleStartDate = resquest.SaleStartDate ?? events.SaleStartDate;
                events.SaleEndDate = resquest.SaleEndDate ?? events.SaleEndDate;
                events.Description = resquest.Description ?? events.Description;
                events.Location = resquest.Location ?? events.Location;
                events.UpdatedDate = DateTime.Now;

                if (resquest.TicketTypes != null)
                {
                    var existingTickets = await _typeTicketRepository.GetByEventIdAsync(EventID);
                    var existingTicketsById = existingTickets.ToDictionary(x => x.Id, x => x);
                    var requestIds = resquest
                        .TicketTypes.Where(x => x.Id.HasValue)
                        .Select(x => x.Id.Value)
                        .ToHashSet();

                    foreach (var ticketRequest in resquest.TicketTypes)
                    {
                        if (ticketRequest.Id.HasValue)
                        {
                            if (
                                !existingTicketsById.TryGetValue(
                                    ticketRequest.Id.Value,
                                    out var ticketEntity
                                )
                            )
                            {
                                return ApiResponse<string>.FailResponse(
                                    Entity.Enum.EnumStatusCode.TYPETICKET,
                                    $"Không tìm thấy loại vé với id {ticketRequest.Id.Value}"
                                );
                            }

                            ticketEntity.Name = ticketRequest.Name;
                            ticketEntity.TotalQuantity = ticketRequest.TotalQuantity!.Value;
                            ticketEntity.Price = ticketRequest.Price!.Value;
                            ticketEntity.Status = ticketRequest.Status!.Value;
                            ticketEntity.UpdatedDate = DateTime.Now;
                            ticketEntity.IsDeleted = false;
                        }
                        else
                        {
                            var newTicket = new TicketType
                            {
                                Name = ticketRequest.Name,
                                TotalQuantity = ticketRequest.TotalQuantity!.Value,
                                Price = ticketRequest.Price!.Value,
                                Status = ticketRequest.Status!.Value,
                                EventID = EventID,
                                IsDeleted = false,
                                CreatedDate = DateTime.Now,
                            };

                            await _typeTicketRepository.CreateTicketType(newTicket);
                        }
                    }

                    var deletedTickets = existingTickets
                        .Where(x => !requestIds.Contains(x.Id))
                        .ToList();

                    foreach (var deletedTicket in deletedTickets)
                    {
                        deletedTicket.IsDeleted = true;
                        deletedTicket.UpdatedDate = DateTime.Now;
                    }
                }

                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();

                if (
                    !string.IsNullOrWhiteSpace(oldImageUrl)
                    && !string.Equals(oldImageUrl, newImageUrl, StringComparison.OrdinalIgnoreCase)
                )
                {
                    _imageService.Delete(oldImageUrl);
                }

                return ApiResponse<string>.SuccessResponse(
                    Entity.Enum.EnumStatusCode.SUCCESS,
                    "Update Thành công"
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"System Error: {ex.Message}");

                if (transactionStarted)
                {
                    await _uow.RollbackAsync();
                }

                if (!string.IsNullOrWhiteSpace(newImageUrl))
                {
                    _imageService.Delete(newImageUrl);
                }

                return ApiResponse<string>.FailResponse(
                    Entity.Enum.EnumStatusCode.SERVER,
                    "lỗi",
                    ex.Message
                );
            }
        }

        public async Task<ApiResponse<string>> DuplicateEvent(Guid eventId)
        {
            var transactionStarted = false;
            try
            {
                var originalEvent = await _eventRepository.GetEventById(eventId);
                if (IsEventNotFound(originalEvent))
                {
                    return ApiResponse<string>.FailResponse(
                        Entity.Enum.EnumStatusCode.EVENTNOTFOUD,
                        "Event không tồn tại"
                    );
                }

                await _uow.BeginTransactionAsync();
                transactionStarted = true;

                var posterUrl = _imageService.CloneImage(originalEvent.PosterUrl);

                var newEvent = new Event
                {
                    Id = Guid.NewGuid(),
                    UserID = originalEvent.UserID,
                    Title = originalEvent.Title + " - nhân bản",
                    Status = EnumStatusEvent.DRAFT,
                    PosterUrl = originalEvent.PosterUrl,
                    StartDate = null,
                    EndDate = null,
                    SaleStartDate = null,
                    SaleEndDate = null,
                    Description = originalEvent.Description,
                    Location = originalEvent.Location,
                    CreatedDate = DateTime.Now,
                    CatetoryID = originalEvent.CatetoryID,
                    IsDeleted = false,
                };

                await _eventRepository.CreateEvent(newEvent);

                var existingTickets = await _typeTicketRepository.GetByEventIdAsync(eventId);
                if (existingTickets != null && existingTickets.Any())
                {
                    var newTicketTypes = existingTickets
                        .Select(ticket => new TicketType
                        {
                            Name = ticket.Name,
                            Price = ticket.Price,
                            TotalQuantity = ticket.TotalQuantity,
                            Status = EnumStatusTickType.STOP,
                            EventID = newEvent.Id,
                            IsDeleted = false,
                            CreatedDate = DateTime.Now,
                        })
                        .ToList();

                    await _typeTicketRepository.CreateRangeTicketTypes(newTicketTypes);
                }

                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();

                return ApiResponse<string>.SuccessResponse(
                    Entity.Enum.EnumStatusCode.SUCCESS,
                    "Nhân bản sự kiện thành công"
                );
            }
            catch (Exception ex)
            {
                if (transactionStarted)
                {
                    await _uow.RollbackAsync();
                }
                Console.WriteLine($"System Error: {ex.Message}");
                return ApiResponse<string>.FailResponse(
                    Entity.Enum.EnumStatusCode.SERVER,
                    "lỗi",
                    ex.Message
                );
            }
        }

        //update status
        private bool checkeDateNull(Event request)
        {
            if (request.StartDate == null | request.EndDate == null)
                return false;
            if (request.SaleStartDate == null || request.SaleEndDate == null)
                return false;
            return true;
        }

        public async Task<ApiResponse<string>> UpdateEventStatus(
            Guid eventId,
            EventStatusUpdateRequest request
        )
        {
            try
            {
                await SyncEndedEventsAsync();
                if (request == null || !request.Status.HasValue)
                {
                    return ApiResponse<string>.FailResponse(
                        Entity.Enum.EnumStatusCode.BAD_REQUEST,
                        "Status không hợp lệ"
                    );
                }

                var events = await _eventRepository.GetEventById(eventId);
                if (IsEventNotFound(events))
                {
                    return ApiResponse<string>.FailResponse(
                        Entity.Enum.EnumStatusCode.EVENTNOTFOUD,
                        "event không tồn tại"
                    );
                }

                await EnsureEventEndedStatusAsync(events);
                if (events.Status == EnumStatusEvent.ENDED)
                {
                    return ApiResponse<string>.FailResponse(
                        Entity.Enum.EnumStatusCode.BAD_REQUEST,
                        "Sự kiện đã kết thúc, không thể cập nhật trạng thái"
                    );
                }

                var check = checkeDateNull(events);
                if (!check)
                {
                    return ApiResponse<string>.FailResponse(
                        Entity.Enum.EnumStatusCode.DATE,
                        "Kiểm tra lại ngày tháng"
                    );
                }

                events.Status = request.Status.Value;
                events.UpdatedDate = DateTime.Now;

                await _uow.SaveChangesAsync();

                return ApiResponse<string>.SuccessResponse(
                    Entity.Enum.EnumStatusCode.SUCCESS,
                    "Cập nhật status event thành công"
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"System Error: {ex.Message}");
                return ApiResponse<string>.FailResponse(
                    Entity.Enum.EnumStatusCode.SERVER,
                    "lỗi",
                    ex.Message
                );
            }
        }

        public async Task<PageResponse<EventTypeTickResponses>> GetPageWithTicketTypes(
            PageRequest query,
            bool isAdmin = false
        )
        {
            await SyncEndedEventsAsync();
            if (query.PageIndex <= 0)
                query.PageIndex = 1;
            if (query.PageSize <= 0)
                query.PageSize = 10;

            return await _eventRepository.GetAllWithTicketTypesAsync(query, isAdmin);
        }

        public async Task<PageResponse<EventTypeTickResponses>> GetPageWithTicketTypesbyId(
            Guid id,
            PageRequest query
        )
        {
            await SyncEndedEventsAsync();
            if (query.PageIndex <= 0)
                query.PageIndex = 1;
            if (query.PageSize <= 0)
                query.PageSize = 10;

            return await _eventRepository.GetAllWithTicketTypesAsyncbyid(id, query);
        }
    }
}
