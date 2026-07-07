using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Common.Exceptions;
using TaskManager.Application.Features.Users.DTOs;
using TaskManager.Application.Features.Roles.DTOs;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Features.Users;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public UserService(IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task<UserDto> CreateAsync(string username, string fullName, string email, string password)
    {
        var user = new User(username, fullName, email, password);

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return MapToDto(user);
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();

        return users
            .Select(MapToDto)
            .ToList();
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if ( user is null )
        {
            return null;
        }

        return MapToDto(user);
    }

    public async Task<UserWithRolesDto?> GetOneWithRolesAsync(Guid userId)
    {
        var user = await _userRepository.GetOneWithPermissionsAsync(userId);

        if (user is null)
        {
            return null;
        }

        var roles = user.UserRoles.Select(ur => new RoleDto
        {
            Id = ur.Role.Id,
            Name = ur.Role.Name,
            Description = ur.Role.Description
        }).ToList();

        return new UserWithRolesDto
        {
            Id = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            Email = user.Email,
            Avatar = user.Avatar is null ? string.Empty : user.Avatar,
            AvatarBg = user.AvatarBg is null ? string.Empty : user.AvatarBg,
            LastSession = user.LastSession,
            CreatedAt = user.CreatedAt,
            IsActive = user.IsActive,
            Roles = roles
        };
    }

    public async Task<bool> UpdateAsync(Guid id, string username, string fullName, string email)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
        {
            return false;
        }

        user.Update(username, fullName, email);

        await _userRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateAllDataAsync(Guid id, string username, string fullName, string email, string avatar, string avatarBg)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
        {
            return false;
        }

        user.UpdateAll(username, fullName, email, avatar, avatarBg);

        await _userRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateAvatarAsync(Guid id, string avatar, string avatarBg)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
        {
            return false;
        }

        user.UpdateAvatar(avatar, avatarBg);

        await _userRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdatePasswordAsync(Guid id, string newPassword)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
        {
            return false;
        }

        user.UpdatePassword(newPassword);

        await _userRepository.SaveChangesAsync();

        return true;
    }

    public async System.Threading.Tasks.Task AssignRoleAsync(Guid userId, Guid roleId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            throw new NotFoundException($"User with ID {userId} not found.");
        }

        var role = await _roleRepository.GetByIdAsync(roleId);

        if (role is null)
        {
            throw new NotFoundException($"Role with ID {roleId} not found.");
        }

        user.AssignRole(roleId);

        await _userRepository.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
        {
            return false;
        }

        user.Delete();

        await _userRepository.SaveChangesAsync();

        return true;
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            Email = user.Email,
            Avatar = user.Avatar is null ? string.Empty : user.Avatar,
            AvatarBg = user.AvatarBg is null ? string.Empty : user.AvatarBg,
            LastSession = user.LastSession,
            CreatedAt = user.CreatedAt,
            IsActive = user.IsActive
        };
    }   

}