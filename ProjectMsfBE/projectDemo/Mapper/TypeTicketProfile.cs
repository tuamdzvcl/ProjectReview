using AutoMapper;
using EventTick.Model.Models;
using projectDemo.DTO.Projection;
using projectDemo.DTO.Request;
using projectDemo.DTO.Response;
using projectDemo.DTO.UpdateRequest;

namespace projectDemo.Mapper
{
    public class TypeTicketProfile : Profile
    {
       
           public TypeTicketProfile() {
            CreateMap<TypeTicketRequest, TicketType>();



            CreateMap<EventProjection, EventTypeTickResponses>()
               .ForMember(dest => dest.Status,
                   opt => opt.MapFrom(src => src.Status.ToString())
               )
               .ForMember(dest => dest.ListTypeTick,
                    opt => opt.MapFrom(src => src.listTypeTick)); 
             

            CreateMap<TicketType, TypeTickResponse>()
                .ForMember(dest => dest.Name,
                    opt => opt.MapFrom(src => src.Name.ToString())
                )
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()));


           
        }

        
    }
}
