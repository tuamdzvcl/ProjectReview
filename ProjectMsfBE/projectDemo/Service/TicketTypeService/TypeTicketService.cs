using AutoMapper;
using EventTick.Model.Enum;
using EventTick.Model.Models;
using projectDemo.DTO.Request;
using projectDemo.DTO.Respone;
using projectDemo.DTO.Response;
using projectDemo.DTO.UpdateRequest;
using projectDemo.Repository;
using projectDemo.Repository.Ipml;
using projectDemo.Repository.TickTypeRepository;
using projectDemo.UnitOfWorks;
using static Dapper.SqlMapper;

namespace projectDemo.Service.TicketTypeService
{
    public class TypeTicketService : ITypeTicketService
    {
        private readonly ITypeTicketRepositorys _ticketRepositorys;
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        public TypeTicketService(
            IUnitOfWork uow,
            ITypeTicketRepositorys ticketRepositorys,
            IMapper mapper,
            IEventRepository eventRepository
        )
        {
            _ticketRepositorys = ticketRepositorys;
            _mapper = mapper;
            _eventRepository = eventRepository;
            _uow = uow;
        }

        //tạo loại vé
        public async Task<ApiResponse<TypeTickResponse>> CreateTypeTickect(
            TypeTicketRequest request
        )
        {
            var EventID = _eventRepository.GetEventById(request.EventID);

            if (EventID == null)
            {
                return ApiResponse<TypeTickResponse>.FailResponse(
                    Entity.Enum.EnumStatusCode.EVENTNOTFOUD,
                    "Không tìm thấy Event ID"
                );
            }

            var ticket = new TicketType
            {
                Name = request.Name,
                SoldQuantity = request.SoldQuantity,
                Price = request.Price,
                Status = request.Status,
                TotalQuantity = request.TotalQuantity,
                IsDeleted = false,
                EventID = request.EventID,
            };
            var entity = await _ticketRepositorys.CreateTicketType(ticket);
            await _uow.SaveChangesAsync();
            var response = _mapper.Map<TypeTickResponse>(entity);
            return ApiResponse<TypeTickResponse>.SuccessResponse(
                Entity.Enum.EnumStatusCode.SUCCESS,
                response
            );
        }

        //xóa loại vé
        public async Task<ApiResponse<string>> DeleteTypeTicket(int TypeTickectID)
        {
            var typeticket = _ticketRepositorys.GetTicketTypebyId(TypeTickectID);
            if (typeticket == null)
            {
                return ApiResponse<string>.FailResponse(
                    Entity.Enum.EnumStatusCode.SERVER,
                    "Không tìm thấy typeTicketID"
                );
            }
            var response = _ticketRepositorys.DeleteTicket(typeticket);
            return ApiResponse<string>.SuccessResponse(Entity.Enum.EnumStatusCode.SERVER, response);
        }

        //lấy tất cả loại vé theo eventID
        public async Task<ApiResponse<EventTypeTickResponses>> GetListTypeTickByEventID(
            Guid eventID
        )
        {
            var (eventData, status, messger) = await _ticketRepositorys.GetListTypeTickByEventID(
                eventID
            );
            if (status != 200)
            {
                return ApiResponse<EventTypeTickResponses>.FailResponse(
                    Entity.Enum.EnumStatusCode.EVENTNOTFOUD,
                    messger
                );
            }
            var ev = _mapper.Map<EventTypeTickResponses>(eventData);
            return ApiResponse<EventTypeTickResponses>.SuccessResponse(
                Entity.Enum.EnumStatusCode.SUCCESS,
                ev
            );
        }

        // lấy vé đầu tiền theo sự kiện
        public async Task<ApiResponse<TypeTickResponse>> GetTypeTickByEevntID(Guid EventID)
        {
            try
            {
                var events = await _eventRepository.GetEventById(EventID);
                if (events == null)
                {
                    return ApiResponse<TypeTickResponse>.FailResponse(
                        Entity.Enum.EnumStatusCode.EVENTNOTFOUD,
                        "Không tìm thấy Event"
                    );
                }
                var typeTick = await _ticketRepositorys.GetTypeTickectByEventID(EventID);
                if (typeTick == null)
                    return ApiResponse<TypeTickResponse>.FailResponse(
                        Entity.Enum.EnumStatusCode.TYPETICKET,
                        "Không tìm thấy Vé hợp lệ"
                    );
                var response = _mapper.Map<TypeTickResponse>(typeTick);

                return ApiResponse<TypeTickResponse>.SuccessResponse(
                    Entity.Enum.EnumStatusCode.SUCCESS,
                    response
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ApiResponse<TypeTickResponse>.FailResponse(
                    Entity.Enum.EnumStatusCode.SERVER,
                    ex.Message
                );
            }
        }

        //sửa thông tin vé sự kiện
        public async Task<ApiResponse<TypeTickResponse>> UpdateTypeTicket(
            int TypeTickectID,
            TypeTicketUpdate request
        )
        {
            if (request == null)
            {
                return ApiResponse<TypeTickResponse>.FailResponse(
                    Entity.Enum.EnumStatusCode.BAD_REQUEST,
                    "Request không hợp lệ"
                );
            }

            var typeticket = _ticketRepositorys.GetTicketTypebyId(TypeTickectID);
            if (typeticket == null)
            {
                return ApiResponse<TypeTickResponse>.FailResponse(
                    Entity.Enum.EnumStatusCode.TYPETICKET,
                    "Không tìm thấy typeTicketID"
                );
            }

            typeticket.Name = request.Name ?? typeticket.Name;
            typeticket.TotalQuantity = request.TotalQuantity ?? typeticket.TotalQuantity;
            typeticket.Price = request.Price ?? typeticket.Price;
            typeticket.SoldQuantity = request.SoldQuantity ?? typeticket.SoldQuantity;
            typeticket.Status = request.Status ?? typeticket.Status;

            var update = _ticketRepositorys.UpdateTicket(typeticket);
            await _uow.SaveChangesAsync();

            var response = _mapper.Map<TypeTickResponse>(update);
            return ApiResponse<TypeTickResponse>.SuccessResponse(
                Entity.Enum.EnumStatusCode.SUCCESS,
                response
            );
        }
    }
}
