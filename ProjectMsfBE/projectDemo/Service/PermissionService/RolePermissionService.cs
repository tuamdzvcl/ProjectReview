using EventTick.Model.Models;
using projectDemo.DTO.Request;
using projectDemo.DTO.Respone;
using projectDemo.DTO.Response;
using projectDemo.Entity.Models;
using projectDemo.Repository.Ipml;
using projectDemo.Repository.PemisstionRepository;
using projectDemo.Repository.RolePermissionRepository;
using projectDemo.UnitOfWorks;

namespace projectDemo.Service.PermissionService
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IUserReposiotry _UserRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPermisstionRepository _permissionRepository;
        private readonly IUnitOfWork _uow;

        public RolePermissionService(
            IRolePermissionRepository rolePermissionRepository,
            IUserReposiotry userRepository,
            IUserRoleRepository userRoleRepository,
            IRoleRepository roleRepository,
            IPermisstionRepository permissionRepository,
            IUnitOfWork uow
        )
        {
            _rolePermissionRepository = rolePermissionRepository;
            _UserRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _uow = uow;
        }

        public async Task<ApiResponse<string>> CreateRolePermission(RolePermissionResquest resquest)
        {
            try
            {
                var roleName = resquest.RoleName?.Trim().ToUpper();
                if (string.IsNullOrEmpty(roleName))
                    return ApiResponse<string>.FailResponse(
                        Entity.Enum.EnumStatusCode.BAD_REQUEST,
                        "Tên Role không được để trống"
                    );

                var existingRole = await _roleRepository.GetRole(roleName);
                if (existingRole != null)
                {
                    return ApiResponse<string>.FailResponse(
                        Entity.Enum.EnumStatusCode.BAD_REQUEST,
                        "Role đã tồn tại trong hệ thống"
                    );
                }

                if (resquest.permissionResquests == null || !resquest.permissionResquests.Any())
                    return ApiResponse<string>.FailResponse(
                        Entity.Enum.EnumStatusCode.BAD_REQUEST,
                        "Vui lòng chọn ít nhất một quyền cho Role"
                    );

                var requestedIds = resquest
                    .permissionResquests.Select(p => p.PermissionId)
                    .Distinct()
                    .ToList();

                var allPermissions = await _permissionRepository.GetAllAsync();
                var existingPermissionIds = allPermissions
                    .Where(p => requestedIds.Contains(p.Id))
                    .Select(p => p.Id)
                    .ToList();

                if (existingPermissionIds.Count != requestedIds.Count)
                {
                    var missingIds = requestedIds.Except(existingPermissionIds).ToList();
                    return ApiResponse<string>.FailResponse(
                        Entity.Enum.EnumStatusCode.BAD_REQUEST,
                        $"Các ID quyền sau không tồn tại: {string.Join(", ", missingIds)}"
                    );
                }

                await _uow.BeginTransactionAsync();

                var newRole = new Role { RoleName = roleName,
                CreatedDate= DateTime.Now
                
                };
                await _roleRepository.GetOrCreateAsync(newRole);
                await _uow.SaveChangesAsync();

                var rolePermissions = requestedIds
                    .Select(pid => new RolePermissions
                    {
                        RoleId = newRole.Id,
                        PermissionId = pid,
                        CreatedDate = DateTime.Now,
                    })
                    .ToList();

                await _rolePermissionRepository.AddRangeAsync(rolePermissions);

                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();

                return ApiResponse<string>.SuccessResponse(
                    Entity.Enum.EnumStatusCode.SUCCESS,
                    "Tạo Role và gán quyền thành công"
                );
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                Console.WriteLine(ex.ToString());
                return ApiResponse<string>.FailResponse(
                    Entity.Enum.EnumStatusCode.SERVER,
                    $"Lỗi hệ thống: {ex.Message}"
                );
            }
        }

        public async Task<ApiResponse<List<PermisstionRoleResponse>>> GetListPermissionRole()
        {
            try
            {
                var result = await _roleRepository.GetRoleListPermisson();
                return ApiResponse<List<PermisstionRoleResponse>>.SuccessResponse(
                    Entity.Enum.EnumStatusCode.SUCCESS,
                    result
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ApiResponse<List<PermisstionRoleResponse>>.FailResponse(
                    Entity.Enum.EnumStatusCode.SERVER,
                    ex.Message
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<PermisstionRoleResponse>>> GetPermissionByRoleId(
            int roleId
        )
        {
            try
            {
                var result = await _rolePermissionRepository.GetPermissionByRoleId(roleId);
                return ApiResponse<IEnumerable<PermisstionRoleResponse>>.SuccessResponse(
                    Entity.Enum.EnumStatusCode.SUCCESS,
                    result
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ApiResponse<IEnumerable<PermisstionRoleResponse>>.FailResponse(
                    Entity.Enum.EnumStatusCode.SERVER,
                    ex.Message
                );
            }
        }

        public async Task<ApiResponse<string>> UpdateRolePermission(
            RolePermissionResquest request,
            int roleid
        )
        {
            Console.WriteLine("=====UpdatePermission================");
            try
            {
                var role = await _roleRepository.GetByid(roleid);
                if (role == null)
                {
                    return ApiResponse<string>.FailResponse(
                        Entity.Enum.EnumStatusCode.BAD_REQUEST,
                        "Role không tìm thấy trong hệ thống"
                    );
                }

                if (
                    role.IsSystem
                    && !string.IsNullOrEmpty(request.RoleName)
                    && request.RoleName.Trim().ToUpper() != role.RoleName.ToUpper()
                )
                {
                    return ApiResponse<string>.FailResponse(
                        Entity.Enum.EnumStatusCode.BAD_REQUEST,
                        "Không thể đổi tên vai trò hệ thống (System Role)"
                    );
                }

                if (!role.IsSystem && !string.IsNullOrEmpty(request.RoleName))
                {
                    var newName = request.RoleName.Trim().ToUpper();
                    if (!newName.Equals(role.RoleName))
                    {
                        var existingRole = await _roleRepository.GetRole(newName);
                        if (existingRole != null && existingRole.Id != roleid)
                        {
                            return ApiResponse<string>.FailResponse(
                                Entity.Enum.EnumStatusCode.BAD_REQUEST,
                                "Tên Role mới đã tồn tại"
                            );
                        }
                        role.RoleName = newName;
                    }
                }

                if (request.permissionResquests == null)
                    request.permissionResquests = new List<PermissionResquest>();

                var requestedIds = request
                    .permissionResquests.Select(p => p.PermissionId)
                    .Distinct()
                    .ToList();

                var currentEntries = await _rolePermissionRepository.GetByRoleIdAsync(roleid);

                var currentPermissionIds = currentEntries.Select(ce => ce.PermissionId).ToList();

                var toRemove = currentEntries
                    .Where(ce => !requestedIds.Contains(ce.PermissionId))
                    .ToList();
                
                var toAddIds = requestedIds.Except(currentPermissionIds).ToList();

                await _uow.BeginTransactionAsync();

                _roleRepository.Update(role);

                if (toRemove.Any())
                {
                    _rolePermissionRepository.RemoveRange(toRemove);
                }

                if (toAddIds.Any())
                {
                    var rolePermissions = toAddIds
                        .Select(pid => new RolePermissions
                        {
                            RoleId = roleid,
                            PermissionId = pid,
                            CreatedDate = DateTime.Now,
                        })
                        .ToList();
                    await _rolePermissionRepository.AddRangeAsync(rolePermissions);
                }

                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();

                return ApiResponse<string>.SuccessResponse(
                    Entity.Enum.EnumStatusCode.SUCCESS,
                    "Cập nhật vai trò và quyền hạn thành công"
                );
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                Console.WriteLine(ex.ToString());
                return ApiResponse<string>.FailResponse(
                    Entity.Enum.EnumStatusCode.SERVER,
                    $"Lỗi khi cập nhật phân quyền: {ex.Message}"
                );
            }
        }
    }
}
