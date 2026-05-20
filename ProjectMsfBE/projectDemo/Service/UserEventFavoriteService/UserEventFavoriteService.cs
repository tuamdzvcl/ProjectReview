using projectDemo.DTO.Respone;
using projectDemo.Entity.Models;
using projectDemo.Repository.Ipml;
using projectDemo.UnitOfWorks;

namespace projectDemo.Service.UserEventFavoriteService
{
    public class UserEventFavoriteService : IUserEventFavoriteService
    {
        private readonly IUserEventFavoriteRepository _favoriteRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<UserEventFavoriteService> _logger;
        private readonly IUnitOfWork _uow;

        public UserEventFavoriteService(IEventRepository eventRepository,ILogger<UserEventFavoriteService> logger,IUserEventFavoriteRepository favoriteRepository, IUnitOfWork uow)
        {
            _logger = logger;
            _eventRepository = eventRepository;
            _favoriteRepository = favoriteRepository;
            _uow = uow;
        }

        public async Task<ApiResponse<List<Guid>>> GetUserFavoriteIdsAsync(Guid userId)
        {
            try
            {
                var favorites = await _favoriteRepository.GetFavoritesByUserIdAsync(userId);
                var ids = favorites.Select(x => x.EventId).ToList();
                
                return new ApiResponse<List<Guid>>
                {
                    Success = true,
                    Message = "Lấy danh sách yêu thích thành công",
                    Data = ids
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<Guid>> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> ToggleFavoriteAsync(Guid userId, Guid eventId)
        {
            try
            {var eventExists = await _eventRepository.GetEventById(eventId);
        if (eventExists == null || eventExists.Id == Guid.Empty)
        {
            return new ApiResponse<bool> { Success = false, Message = "Sự kiện không tồn tại hoặc đã bị xóa!" };
        }
                var isFavorite = await _favoriteRepository.IsFavoriteAsync(userId, eventId);
                if (isFavorite)
                {
                    await _favoriteRepository.RemoveFavoriteAsync(userId, eventId);
                }
                else
                {
                    await _favoriteRepository.AddFavoriteAsync(new UserEventFavorite
                    {
                        UserId = userId,
                        EventId = eventId,
                        CreateDt = DateTime.Now
                    });
                }

                await _uow.SaveChangesAsync();

                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = isFavorite ? "Đã bỏ yêu thích" : "Đã thêm vào yêu thích",
                    Data = !isFavorite
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }
    }
}
