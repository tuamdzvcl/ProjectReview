using System;
using System.Net.WebSockets;
using AutoMapper;
using EventTick.Model.Enum;
using EventTick.Model.Models;
using projectDemo.DTO.Request;
using projectDemo.DTO.Respone;
using projectDemo.DTO.Response;
using projectDemo.DTO.UpdateRequest;
using projectDemo.Repository;
using projectDemo.Repository.Ipml;
using projectDemo.Repository.OrderRepository;
using projectDemo.Repository.PemisstionRepository;
using projectDemo.Repository.RolePermissionRepository;
using projectDemo.Repository.TickTypeRepository;
using projectDemo.UnitOfWorks;

namespace projectDemo.Service.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserReposiotry _userReposiotry;
        private readonly IMapper _mapper;
        private readonly IUserRoleRepository _roleUserRepo;
        private readonly IRoleRepository _roleRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IPemisstionRepository _petRepository;
        private readonly IUserLoginRepository _userLoginRepository;
        private readonly IRolePermissionRepository _rolePermission;
        private readonly IUnitOfWork _uow;

        public UserService(
            IOrderRepository order,
            IEventRepository eventrp,
            IUserRoleRepository userRole,
            IUnitOfWork uow,
            IUserReposiotry userReposiotry,
            IMapper mapper,
            IRoleRepository roleRepository,
            IPemisstionRepository petRepository,
            IUserLoginRepository userLoginRepository,
            IRolePermissionRepository rolePermission
        )
        {
            _userReposiotry = userReposiotry;
            _mapper = mapper;
            _roleRepository = roleRepository;
            _petRepository = petRepository;
            _userLoginRepository = userLoginRepository;
            _rolePermission = rolePermission;
            _uow = uow;
            _roleUserRepo = userRole;
            _eventRepository = eventrp;
            _orderRepository = order;
        }

        // lấy tất cả các e event do user tạo
        public async Task<ApiResponse<UserEventsResponse>> GetListEventByUserID(Guid guid)
        {
            try
            {
                var (user, events, status, messger) = await _userReposiotry.GetListEventByUserID(
                    guid
                );
                if (status != 200)
                {
                    return ApiResponse<UserEventsResponse>.FailResponse(
                        Entity.Enum.EnumStatusCode.EVENTNOTFOUD,
                        messger
                    );
                }
                var response = new UserEventsResponse
                {
                    User = _mapper.Map<UserResponse>(user),
                    Events = _mapper.Map<List<EventResponse>>(events),
                };

                return ApiResponse<UserEventsResponse>.SuccessResponse(
                    Entity.Enum.EnumStatusCode.SUCCESS,
                    response
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ApiResponse<UserEventsResponse>.FailResponse(
                    Entity.Enum.EnumStatusCode.SUCCESS,
                    "thích làm test lỗi không"
                );
            }
        }

        public async Task<ApiResponse<UserProfile>> GetListEventByUserIDCreate(Guid guid)
        {
            try
            {
                var (user, events, status, messger) = await _userReposiotry.GetListEventByUserID(
                    guid
                );
                if (status != 200)
                {
                    return ApiResponse<UserProfile>.FailResponse(
                        Entity.Enum.EnumStatusCode.EVENTNOTFOUD,
                        messger
                    );
                }
                var response = new UserProfile
                {
                    User = new UserResponseProfile
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        AvatarUrl = user.AvatarUrl,
                        RoleName = user.UserRoles.Select(x => x.Role.RoleName).ToList(),
                    },
                    Events = _mapper.Map<List<EventResponse>>(events),
                };

                return ApiResponse<UserProfile>.SuccessResponse(
                    Entity.Enum.EnumStatusCode.SUCCESS,
                    response
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ApiResponse<UserProfile>.FailResponse(
                    Entity.Enum.EnumStatusCode.SUCCESS,
                    "thích làm test lỗi không"
                );
            }
        }

        //tạo
        async Task<ApiResponse<UserResponse>> IUserService.Create(UserRequest request, Guid userid)
        {
            await _uow.BeginTransactionAsync();
            try
            {
                var user = await _userReposiotry.GetUserByid(userid);

                if (user == null)
                {
                    return ApiResponse<UserResponse>.FailResponse(
                        Entity.Enum.EnumStatusCode.USERNOTFOUND,
                        "không tìm thấy user"
                    );
                }
                var passwordhash = BCrypt.Net.BCrypt.HashPassword("123456");

                var entity = new User
                {
                    Id = Guid.NewGuid(),
                    Username = request.UserName,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    AvatarUrl = request.AvataUrl,
                    IsActive = true,
                    IsLock = false,
                    CreatedDate = DateTime.UtcNow,
                    DateLock = null,
                    Isfalse = 0,
                    PasswordHash = passwordhash,
                    CreatedBy = user.Username,
                };

                await _userReposiotry.Create(entity);
                var Listrole = new List<int>();
                foreach (var item in request.RoleName)
                {
                    var role = await _roleRepository.GetRole(item.ToUpper());
                    if (role == null)
                    {
                        continue;
                    }
                    Listrole.Add(role.Id);
                }
                var userRoles = Listrole
                    .Distinct()
                    .Select(x => new UserRole { UserId = entity.Id, RoleId = x })
                    .ToList();
                await _roleUserRepo.InserList(userRoles);

                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();

                var response = new UserResponse
                {
                    ID = entity.Id,
                    Username = entity.Username,
                    Email = entity.Email,
                    FirstName = entity.FirstName,
                    LastName = entity.LastName,
                    RoleName = request.RoleName,
                };

                return ApiResponse<UserResponse>.SuccessResponse(
                    Entity.Enum.EnumStatusCode.SUCCESS,
                    response
                );
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                return ApiResponse<UserResponse>.FailResponse(
                    Entity.Enum.EnumStatusCode.SERVER,
                    ex.Message
                );
            }
        }

        //xóa
        async Task<ApiResponse<string>> IUserService.Delete(Guid id)
        {
            var user = await _userReposiotry.GetUserByid(id);
            if (user == null)
            {
                return ApiResponse<string>.FailResponse(
                    Entity.Enum.EnumStatusCode.NOT_FOUND,
                    "không tìm thấy user"
                );
            }

            var entitys = await _userReposiotry.GetListEventByUserID(id);

            var hasOrders = await _orderRepository.HasOrderByUserId(user.Id);
            var hasEvents = entitys.status == 200 && entitys.events.Any();

            if (hasEvents || hasOrders)
            {
                user.IsDeleted = true;
            }
            else
            {
                _userReposiotry.Delete(user);
            }
            await _uow.SaveChangesAsync();
            return ApiResponse<string>.SuccessResponse(
                Entity.Enum.EnumStatusCode.SUCCESS,
                "xóa thành công"
            );
        }

        //get all->done
        async Task<PageResponse<UserResponse>> IUserService.GetAll(UserQuery param)
        {
            if (param.PageIndex < 1)
                param.PageIndex = 1;
            if (param.PageSize < 10)
                param.PageSize = 10;
            var (users, total) = await _userReposiotry.GetAll(
                param.PageIndex,
                param.PageSize,
                param.Keyword,
                param.Role
            );

            var response = users
                .Select(x => new UserResponse
                {
                    ID = x.Id,
                    Username = x.Username,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    RoleName = x.UserRoles.Select(x => x.Role.RoleName.ToLower()).ToList(),
                })
                .ToList();

            var page = new PageResponse<UserResponse>
            {
                Items = response,
                PageIndex = param.PageIndex,
                PageSize = param.PageSize,
                TotalRecords = total,
                Success = true,
                Message = "list user",
            };

            return page;
        }

        //update
        async Task<ApiResponse<UserResponse>> IUserService.Update(
            Guid id,
            UserUpdateRequest request
        )
        {
            if (request == null)
            {
                return ApiResponse<UserResponse>.FailResponse(
                    Entity.Enum.EnumStatusCode.BAD_REQUEST,
                    "Request is null"
                );
            }
            var user = await _userReposiotry.GetUserByid(id);
            if (user == null)
            {
                return ApiResponse<UserResponse>.FailResponse(
                    Entity.Enum.EnumStatusCode.NOT_FOUND,
                    "KHông tìm thấy User"
                );
            }

            user.FirstName = request.FirstName?.Trim() ?? user.FirstName;
            user.LastName = request.LastName?.Trim() ?? user.LastName;
            user.AvatarUrl = request.AvataUrl ?? user.AvatarUrl;
            user.UpdatedDate = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(request.RoleName))
            {
                var role = await _roleRepository.GetRole(request.RoleName);
                if (role == null)
                {
                    return ApiResponse<UserResponse>.FailResponse(
                        Entity.Enum.EnumStatusCode.NOT_FOUND,
                        "Role không tồn tại"
                    );
                }

                user.UserRoles.Clear();
                user.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id });
            }
            await _uow.SaveChangesAsync();

            var userReponse = new UserResponse
            {
                AvatarUrl = user.AvatarUrl,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
            };
            return ApiResponse<UserResponse>.SuccessResponse(
                Entity.Enum.EnumStatusCode.SUCCESS,
                userReponse
            );
        }
    }
}
