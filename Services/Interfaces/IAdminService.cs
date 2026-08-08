using AcademicAppoinment.DTOs.Admin;
using System.Security.Claims;

namespace AcademicAppoinment.Services.Interfaces
{
    public interface IAdminService
    {
        Task<IReadOnlyList<AdminUserListItemDto>> GetUsersAsync();
        Task<AdminUserDetailDto> GetUserByIdAsync(int userId);
        Task<IReadOnlyList<RoleDto>> GetRolesAsync();
        Task<bool> SetUserActiveAsync(int userId, bool isActive, ClaimsPrincipal currentUser);
        Task<bool> SetUserRoleAsync(int userId, int roleId, ClaimsPrincipal currentUser);
    }
}
