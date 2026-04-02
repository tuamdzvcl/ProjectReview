using AutoMapper;
using EventTick.Model.Models;
using projectDemo.DTO.Projection;
using projectDemo.DTO.Request;
using projectDemo.DTO.Response;
using projectDemo.DTO.UpdateRequest;
using projectDemo.Entity.Models;

namespace projectDemo.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            CreateMap<EventRequest, Event>()
           .ForMember(dest => dest.Id,
            opt => opt.MapFrom(_ => Guid.NewGuid())
            );

            // Entity -> Response
            CreateMap<Event, EventResponse>()
                .ForMember(dest => dest.EventID,
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()));
            CreateMap<User, UserResponse>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.RoleName,
                    opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.RoleName.ToLower()).ToList()));
            CreateMap<EventUpdateRequest, Event>();
            CreateMap<Order, OrderResponse>();
            CreateMap<CreateOrderRequest, OrderDetail>();
            CreateMap<OrderUpdate, Order>();
            CreateMap<PermisstionRequest, Permissions>();
            CreateMap<Permissions, PermissionResponse>();

            CreateMap<TicketType, TypeTickResponse>()
           .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name.ToString()))
           .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
           .ForMember(d => d.Price, opt => opt.MapFrom(s => s.Price));

            CreateMap<Event, EventTypeTickResponses>()
           .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
           .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
           .ForMember(d => d.ListTypeTick, opt => opt.MapFrom(s => s.TicketTypes))
           .ForMember(d => d.CatetoryName, opt => opt.MapFrom(s => s.Catetory.Name));
        }
    }
}
