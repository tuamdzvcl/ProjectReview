using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using AutoMapper;
using Azure;
using Microsoft.Extensions.Logging;
using projectDemo.DTO.Request;
using projectDemo.DTO.Respone;
using projectDemo.DTO.Response;
using projectDemo.Entity.Models;
using projectDemo.Repository.Ipml;
using projectDemo.Repository.PemisstionRepository;
using projectDemo.UnitOfWorks;

namespace projectDemo.Service.PermissionService
{
    public class PermissionService : IPermissionService
    {
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;
        private readonly IPermisstionRepository _pemisstionRepository;
        private readonly IUserReposiotry _userReposiotry;
        private readonly IUnitOfWork _uow;

        public PermissionService(
            IMapper mapper,
            IRoleRepository roleRepository,
            IPermisstionRepository pemisstionRepository,
            IUserReposiotry userReposiotry
        )
        {
            _mapper = mapper;
            _roleRepository = roleRepository;
            _pemisstionRepository = pemisstionRepository;
            _userReposiotry = userReposiotry;
        }

        public async Task<ApiResponse<PermissionResponse>> Create(PermisstionRequest request)
        {
            var entity = _mapper.Map<Permissions>(request);
            await _pemisstionRepository.Create(entity);
            await _uow.SaveChangesAsync();
            var response = new PermissionResponse
            {
                PermissonsDescription = entity.PermissonsDescription,
                PermissonsName = entity.PermissonsName,
            };
            return ApiResponse<PermissionResponse>.SuccessResponse(
                Entity.Enum.EnumStatusCode.SUCCESS,
                response
            );
        }

        public async Task<ApiResponse<string>> Delete(int permissionID)
        {
            var per = await _pemisstionRepository.GetByID(permissionID);
            if (per == null)
            {
                return ApiResponse<string>.FailResponse(
                    Entity.Enum.EnumStatusCode.SERVER,
                    "Không tìm thấy quyền xóa"
                );
            }
            per.IsDeleted = true;
            per.UpdatedDate = DateTime.UtcNow;

            await _uow.SaveChangesAsync();
            return ApiResponse<string>.SuccessResponse(
                Entity.Enum.EnumStatusCode.SUCCESS,
                "xóa thành công"
            );
        }

        public async Task<ApiResponse<PermissionResponse>> GetByID(int permissionID)
        {
            var entity = await _pemisstionRepository.GetByID(permissionID);
            if (entity == null)
                return ApiResponse<PermissionResponse>.FailResponse(
                    Entity.Enum.EnumStatusCode.SERVER,
                    "Không tìm thấy quyền xóa"
                );
            var response = new PermissionResponse
            {
                PermissonsDescription = entity.PermissonsDescription,

                PermissonsName = entity.PermissonsName,
            };

            return ApiResponse<PermissionResponse>.SuccessResponse(
                Entity.Enum.EnumStatusCode.SUCCESS,
                response
            );
        }

        public async Task<ApiResponse<RoleResponse>> GetByrole(int RoleId)
        {
            try
            {
                var (role, status, messger) = await _pemisstionRepository.GetPermissionByRoleId(
                    RoleId
                );
                if (status != 200)
                {
                    return ApiResponse<RoleResponse>.FailResponse(
                        Entity.Enum.EnumStatusCode.EVENTNOTFOUD,
                        messger
                    );
                }

                var response = new RoleResponse
                {
                    RoleName = role.RoleName.ToString(),
                    permissions = role
                        .ListPermissions.Select(p => new PermissionResponse
                        {
                            PermissonsDescription = p.PermissonsDescription,
                            PermissonsName = p.PermissonsName,
                        })
                        .ToList(),
                };
                return ApiResponse<RoleResponse>.SuccessResponse(
                    Entity.Enum.EnumStatusCode.SUCCESS,
                    response
                );
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                return ApiResponse<RoleResponse>.FailResponse(
                    Entity.Enum.EnumStatusCode.SERVER,
                    ex.Message
                );
            }
        }

        public async Task<ApiResponse<List<PermissionResponse>>> GetListPermission()
        {
            
            var permission = await _pemisstionRepository.GetAllAsync();
            if (permission == null)
            {
                return ApiResponse<List<PermissionResponse>>.FailResponse(
                    Entity.Enum.EnumStatusCode.BAD_REQUEST,
                    "danh sách chưa có quyền nào"
                );
            }

            var response = permission
                .Select(p => new PermissionResponse
                {
                    Id = p.Id,
                    PermissonsName = p.PermissonsName,
                    PermissonsDescription = p.PermissonsDescription,
                })
                .ToList();

            return ApiResponse<List<PermissionResponse>>.SuccessResponse(
                Entity.Enum.EnumStatusCode.SUCCESS,
                response
            );
        }

        public Task<ApiResponse<PermissionResponse>> Update(PermisstionRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
