using TaskManager.Application.Features.Users.DTOs;

namespace TaskManager.Application.Features.Users;

public interface IUserService
{
    Task<UserDto> CreateAsync(string username, string fullName, string email, string password);

    Task<List<UserDto>> GetAllAsync();

    Task<UserDto?> GetByIdAsync(Guid id);

    Task<UserWithRolesDto?> GetOneWithRolesAsync(Guid userId);

    Task<bool> UpdateAsync(Guid id, string username, string fullName, string email);

    Task<bool> UpdateAllDataAsync(Guid id, string username, string fullName, string email, string avatar, string avatarBg);

    Task<bool> UpdateAvatarAsync(Guid id, string avatar, string avatarBg);

    Task<bool> UpdatePasswordAsync(Guid id, string newPassword);

    Task AssignRoleAsync(Guid userId, Guid roleId);

    Task AssignManyRolesAsync(Guid userId, List<Guid> roleIds);

    Task<bool> DeleteAsync(Guid id);
}