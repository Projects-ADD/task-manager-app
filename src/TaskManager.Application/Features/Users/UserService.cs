using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Common.Exceptions;
using TaskManager.Application.Features.Users.DTOs;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Features.Users;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
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
            Avatar = user.Avatar,
            AvatarBg = user.AvatarBg,
            LastSession = user.LastSession,
            CreatedAt = user.CreatedAt,
            IsActive = user.IsActive
        };
    }   

}