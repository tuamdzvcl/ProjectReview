
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
using projectDemo.Repository.TickTypeRepository;
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
        private readonly ITypeTicketRepositorys _typeTicketRepository;
        private readonly IMapper _mapper;

        public EventService(ICatetoryReposioty catetoryReposioty,IUnitOfWork uow,IImageService imageService,IEventRepository eventRepository, IMapper mapper, IUserReposiotry userReposiotry, ITypeTicketRepositorys typeTicketRepository)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
            _userReposiotry = userReposiotry;
            _imageService = imageService;
            _uow = uow;
            _catrtoeyRepository = catetoryReposioty;
            _typeTicketRepository = typeTicketRepository;
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

        private static bool IsEventNotFound(Event events)
        {
            return events == null || events.Id == Guid.Empty;
        }

        private static EventRequest BuildEventValidationRequest(Event existingEvent, EventUpdateRequest request)
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
                PosterUrl = request.PosterUrl
            };
        }

        private static bool HasInvalidTicketTypes(IEnumerable<CreateEventTicketTypeItemRequest> ticketTypes)
        {
            return ticketTypes.Any(ticket =>
                ticket.TotalQuantity <= 0 ||
                ticket.SoldQuantity < 0 ||
                ticket.SoldQuantity > ticket.TotalQuantity ||
                ticket.Price <= 0);
        }

        private static bool HasInvalidTicketTypeUpdates(IEnumerable<UpdateEventTicketTypeItemRequest> ticketTypes)
        {
            return ticketTypes.Any(ticket =>
                !ticket.Name.HasValue ||
                !ticket.TotalQuantity.HasValue ||
                !ticket.Price.HasValue ||
                !ticket.SoldQuantity.HasValue ||
                !ticket.Status.HasValue ||
                ticket.TotalQuantity <= 0 ||
                ticket.SoldQuantity < 0 ||
                ticket.SoldQuantity > ticket.TotalQuantity ||
                ticket.Price <= 0);
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

        public async Task<ApiResponse<CreateEventWithTicketTypesResponse>> CreateEventWithTicketTypes(CreateEventWithTicketTypesRequest request, Guid userId)
        {
            string? imageUrl = null;
            var transactionStarted = false;

            try
            {
                if (!checkVadidate(request))
                {
                    return ApiResponse<CreateEventWithTicketTypesResponse>.FailResponse(Entity.Enum.EnumStatusCode.DATE, "Kiểm tra lại ngày giờ của event");
                }

                if (request.TicketTypes == null || !request.TicketTypes.Any())
                {
                    return ApiResponse<CreateEventWithTicketTypesResponse>.FailResponse(Entity.Enum.EnumStatusCode.BAD_REQUEST, "Event phải có ít nhất 1 loại vé");
                }

                if (HasInvalidTicketTypes(request.TicketTypes))
                {
                    return ApiResponse<CreateEventWithTicketTypesResponse>.FailResponse(Entity.Enum.EnumStatusCode.BAD_REQUEST, "Thông tin loại vé không hợp lệ");
                }

                if (request.PosterUrl == null)
                {
                    return ApiResponse<CreateEventWithTicketTypesResponse>.FailResponse(Entity.Enum.EnumStatusCode.BAD_REQUEST, "PosterUrl không được để trống");
                }

                var user = await _userReposiotry.GetUserByid(userId);
                if (user == null)
                {
                    return ApiResponse<CreateEventWithTicketTypesResponse>.FailResponse(Entity.Enum.EnumStatusCode.USERNOTFOUND, "Không tìm thấy user");
                }

                var catetory = await _catrtoeyRepository.GetByName(request.CatetoryName.ToUpper());
                if (catetory == null)
                {
                    return ApiResponse<CreateEventWithTicketTypesResponse>.FailResponse(Entity.Enum.EnumStatusCode.NOT_FOUND, "Không tìm thấy thể loại");
                }

                imageUrl = await _imageService.UploadAsync(request.PosterUrl);

                await _uow.BeginTransactionAsync();
                transactionStarted = true;

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
                    IsDeleted = false
                };

                var ticketTypeEntities = request.TicketTypes.Select(ticket => new TicketType
                {
                    Name = ticket.Name,
                    Price = ticket.Price,
                    TotalQuantity = ticket.TotalQuantity,
                    SoldQuantity = ticket.SoldQuantity,
                    Status = ticket.Status,
                    EventID = eventEntity.Id,
                    IsDeleted = false,
                    CreatedDate = DateTime.Now
                }).ToList();

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
                    TicketTypes = ticketTypeEntities.Select(ticket => new TypeTickResponse
                    {
                        Id = ticket.Id,
                        Name = ticket.Name.ToString(),
                        Price = ticket.Price,
                        TotalQuantity = ticket.TotalQuantity,
                        SoldQuantity = ticket.SoldQuantity,
                        Status = ticket.Status.ToString()
                    }).ToList()
                };

                return ApiResponse<CreateEventWithTicketTypesResponse>.SuccessResponse(Entity.Enum.EnumStatusCode.SUCCESS, response);
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

                return ApiResponse<CreateEventWithTicketTypesResponse>.FailResponse(Entity.Enum.EnumStatusCode.SERVER, "Lỗi khi tạo event", ex.Message);
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
            string? oldImageUrl = null;
            string? newImageUrl = null;
            var transactionStarted = false;
            try
            {
                var events = await _eventRepository.GetEventById(EventID);
                if (IsEventNotFound(events))
                    return ApiResponse<string>.FailResponse(Entity.Enum.EnumStatusCode.EVENTNOTFOUD, "even không tồn tại ");

                var validationRequest = BuildEventValidationRequest(events, resquest);
                if (!checkVadidate(validationRequest))
                {
                    return ApiResponse<string>.FailResponse(Entity.Enum.EnumStatusCode.DATE, "Kiểm tra lại ngày giờ của event");
                }

                if (resquest.TicketTypes != null && resquest.TicketTypes.Any() && HasInvalidTicketTypeUpdates(resquest.TicketTypes))
                {
                    return ApiResponse<string>.FailResponse(Entity.Enum.EnumStatusCode.BAD_REQUEST, "Thông tin loại vé không hợp lệ");
                }

                await _uow.BeginTransactionAsync();
                transactionStarted = true;

                if (!string.IsNullOrWhiteSpace(resquest.CatetoryName))
                {
                    var catetory = await _catrtoeyRepository.GetByName(resquest.CatetoryName.ToUpper());
                    if (catetory == null)
                    {
                        return ApiResponse<string>.FailResponse(Entity.Enum.EnumStatusCode.NOT_FOUND, "Không tìm thấy thể loại");
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
                    var requestIds = resquest.TicketTypes
                        .Where(x => x.Id.HasValue)
                        .Select(x => x.Id!.Value)
                        .ToHashSet();

                    foreach (var ticketRequest in resquest.TicketTypes)
                    {
                        if (ticketRequest.Id.HasValue)
                        {
                            if (!existingTicketsById.TryGetValue(ticketRequest.Id.Value, out var ticketEntity))
                            {
                                return ApiResponse<string>.FailResponse(Entity.Enum.EnumStatusCode.TYPETICKET, $"Không tìm thấy loại vé với id {ticketRequest.Id.Value}");
                            }

                            ticketEntity.Name = ticketRequest.Name!.Value;
                            ticketEntity.TotalQuantity = ticketRequest.TotalQuantity!.Value;
                            ticketEntity.Price = ticketRequest.Price!.Value;
                            ticketEntity.SoldQuantity = ticketRequest.SoldQuantity!.Value;
                            ticketEntity.Status = ticketRequest.Status!.Value;
                            ticketEntity.UpdatedDate = DateTime.Now;
                            ticketEntity.IsDeleted = false;
                        }
                        else
                        {
                            var newTicket = new TicketType
                            {
                                Name = ticketRequest.Name!.Value,
                                TotalQuantity = ticketRequest.TotalQuantity!.Value,
                                Price = ticketRequest.Price!.Value,
                                SoldQuantity = ticketRequest.SoldQuantity!.Value,
                                Status = ticketRequest.Status!.Value,
                                EventID = EventID,
                                IsDeleted = false,
                                CreatedDate = DateTime.Now
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
                        deletedTicket.Status = EnumStatusTickType.STOP;
                        deletedTicket.UpdatedDate = DateTime.Now;
                    }
                }

                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();

                if (!string.IsNullOrWhiteSpace(oldImageUrl) && !string.Equals(oldImageUrl, newImageUrl, StringComparison.OrdinalIgnoreCase))
                {
                    _imageService.Delete(oldImageUrl);
                }

                return ApiResponse<string>.SuccessResponse(Entity.Enum.EnumStatusCode.SUCCESS, "Update Thành công");
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
