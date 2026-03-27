using Google.Apis.Util;
using projectDemo.DTO.Request;
using projectDemo.DTO.Respone;
using projectDemo.DTO.Response;
using projectDemo.Entity.Models;
using projectDemo.Repository.CatetoryRepository;
using projectDemo.Repository.Ipml;
using projectDemo.UnitOfWorks;

namespace projectDemo.Service.CatetoryService
{
    public class CatetoryService : ICatetoryService
    {
        private readonly ICatetoryReposioty _catetoryReposioty;
        private readonly IEventRepository _eventRepository;
        private readonly IUnitOfWork _uow;

        public CatetoryService(IUnitOfWork uow,ICatetoryReposioty catetoryReposioty, IEventRepository eventRepository)
        {
            _catetoryReposioty = catetoryReposioty;
            _eventRepository = eventRepository;
            _uow = uow;
        }

        public async Task<ApiResponse<CatetoryResponse>> Create(CatetoryResquest resquest)
        {
            Catetory catetory = new Catetory
            {
                Id = Guid.NewGuid(),
                Name = resquest.Name
            };

            await _catetoryReposioty.Create(catetory);
           await _uow.SaveChangesAsync();

            CatetoryResponse response = new CatetoryResponse
            {
                Name = catetory.Name
            };


            return ApiResponse<CatetoryResponse>.SuccessResponse(Entity.Enum.EnumStatusCode.SUCCESS, response);
        }

        public async Task<ApiResponse<string>> Delete(Guid id)
        {
           try
            {
                await _catetoryReposioty.delete(id);
                await _uow.SaveChangesAsync();
                return ApiResponse<string>.SuccessResponse(Entity.Enum.EnumStatusCode.SUCCESS, "Xóa thành công");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());

                return ApiResponse<string>.FailResponse(Entity.Enum.EnumStatusCode.SUCCESS,$"đọc lỗi đi {ex.ToString()}");

            }
        }

        public async Task<ApiResponse<CatetoryResponse>> Getbyid(Guid id)
        {
            var catetory =  await _catetoryReposioty.GetbyId(id);
            var response = new CatetoryResponse
            {
                Name = catetory.Name,
                listEvent = catetory.Events.Select(c => new EventResponse
                {
                    Description = c.Description,
                    EndDate = c.EndDate,
                    EventID = c.Id,
                    Location = c.Location,
                    PosterUrl = c.PosterUrl,
                    SaleEndDate = c.SaleEndDate,

                    SaleStartDate = c.SaleStartDate,
                    StartDate = c.StartDate,
                    Status = c.Status.ToString(),
                    Title = c.Title,
                    UserID = c.UserID
                }).ToList()

            };
            return ApiResponse<CatetoryResponse>.SuccessResponse(Entity.Enum.EnumStatusCode.SUCCESS, response);
        }

        public async Task<ApiResponse<List<CatetoryResponse>>> GetCatetory()
        {
            var items = await _catetoryReposioty.GetListCatetory();

            var response = items.Select(x => new CatetoryResponse
            {
                CatetoryId = x.Id,
                Name = x.Name
            }).ToList();

            return ApiResponse<List<CatetoryResponse>>
                .SuccessResponse(Entity.Enum.EnumStatusCode.SUCCESS, response);
        }

        public async Task<PageResponse<CatetoryResponse>> GetCatetoryListEvent(int pageSize, int pageIndex, string key)
        {
            if(pageSize <=0) pageSize = 10;
            if (pageIndex <= 0) pageIndex = 1;
           return await _catetoryReposioty.PageCatetoryEvent(pageSize, pageIndex, key);

        }

        public async Task<ApiResponse<CatetoryResponse>> Update(Guid id, CatetoryResquest resquest)
        { 
 
        var catetory =await _catetoryReposioty.GetbyId(id);
            catetory.Name = resquest.Name?? catetory.Name;
            var response = new CatetoryResponse
            {
                Name = catetory.Name,
                listEvent = catetory.Events.Select(c => new EventResponse
                {
                    Description = c.Description,
                    EndDate = c.EndDate,
                    EventID = c.Id,
                    Location = c.Location,
                    PosterUrl = c.PosterUrl,
                    SaleEndDate = c.SaleEndDate,

                    SaleStartDate = c.SaleStartDate,
                    StartDate = c.StartDate,
                    Status = c.Status.ToString(),
                    Title = c.Title,
                    UserID = c.UserID
                }).ToList()

            };
            return ApiResponse<CatetoryResponse>.SuccessResponse(Entity.Enum.EnumStatusCode.SUCCESS, response);
        }
    }
}