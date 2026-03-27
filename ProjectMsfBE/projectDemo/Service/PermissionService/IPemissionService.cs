using projectDemo.DTO.Request;
using projectDemo.DTO.Respone;
using projectDemo.DTO.Response;

namespace projectDemo.Service.PermissionService
{
    public interface IPemissionService
    {
        Task<ApiResponse<PermissionResponse>> GetByID(int permissionID);
        Task<ApiResponse<RoleResponse>> GetByrole(int RoleId);
        Task<ApiResponse<PermissionResponse>> Create(PermisstionRequest request);
        Task<ApiResponse<PermissionResponse>> Update(PermisstionRequest request);
        Task<ApiResponse<string>> Delete(int permissionID);
        

    }
}
