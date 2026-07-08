using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaskManager.Application.Features.Users;
using TaskManager.Application.Features.Users.DTOs;
using TaskManager.Contracts.Requests;
using TaskManager.Contracts.Responses;

namespace TaskManager.Api.Controllers;

[ApiController]
//[Route("api/[controller]")]
[Route("api/users")]
[Tags("Users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <remarks>
    /// Example request body:
    /// <code>
    /// {
    ///     "username": "johndoe",
    ///     "fullName": "John Doe",
    ///     "email": "johndoe@example.com",
    ///     "password": "password123"
    /// }
    /// </code>
    /// </remarks>
    /// <param name="request">User data: <c>username</c>, <c>fullName</c>, <c>email</c> and <c>password</c>.</param>
    /// <returns>The newly created user wrapped in an API response.</returns>
    /// <response code="201">Returns the created user.</response>

    //TODO: Check how to handle the password in the request.
    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create([FromBody] CreateUserRequest request)
    {
        var user = await _userService.CreateAsync(request.Username, request.FullName, request.Email, request.Password);

        var response = new ApiResponse<UserResponse>
        {
            Action = "post",
            HttpStatusCode = (int)HttpStatusCode.Created,
            Message = "User created successfully.",
            Data = MapToResponse(user)
        };

        return CreatedAtAction(
            nameof(GetById),
            new { id = user.Id },
            response
        );
    }

    /// <summary>
    /// Retrieves a list of all active users.
    /// </summary>
    /// <returns>A list of users wrapped in an API response.</returns>
    /// <response code="200">Returns the list of users.</response>
    /// <response code="404">No users found.</response>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();

        if (users == null || !users.Any())
        {
            return NotFound(new ApiResponse<object>
            {
                Action = "get",
                HttpStatusCode = (int)HttpStatusCode.NotFound,
                Message = "No users found.",
                Data = null
            });
        }

        var response = new ApiResponse<List<UserResponse>>
        {
            Action = "get",
            HttpStatusCode = (int)HttpStatusCode.OK,
            Message = "Users retrieved successfully.",
            Data = users.Select(MapToResponse).ToList()
        };

        return Ok(response);
    }

    /// <summary>
    /// Retrieves a user by their unique identifier, optionally including their assigned roles.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="showRoles">When <c>true</c>, includes the roles assigned to the user.</param>
    /// <returns>The user details wrapped in an API response.</returns>
    /// <response code="200">Returns the user details.</response>
    /// <response code="404">User not found.</response>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, [FromQuery] bool showRoles = false)
    {
        if (showRoles)
        {
            var userWithRoles = await _userService.GetOneWithRolesAsync(id);

            if (userWithRoles is null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Action = "get",
                    HttpStatusCode = (int)HttpStatusCode.NotFound,
                    Message = "User not found.",
                    Data = null
                });
            }

            var responseWithRoles = new ApiResponse<UserWithRolesResponse>
            {
                Action = "get",
                HttpStatusCode = (int)HttpStatusCode.OK,
                Message = "User with roles retrieved successfully.",
                Data = new UserWithRolesResponse
                {
                    Id = userWithRoles.Id,
                    Username = userWithRoles.Username,
                    FullName = userWithRoles.FullName,
                    Email = userWithRoles.Email,
                    Avatar = userWithRoles.Avatar,
                    AvatarBg = userWithRoles.AvatarBg,
                    LastSession = userWithRoles.LastSession,
                    CreatedAt = userWithRoles.CreatedAt,
                    IsActive = userWithRoles.IsActive,
                    Roles = userWithRoles.Roles.Select(r => new RoleResponse
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Description = r.Description
                    }).ToList()
                }
            };

            return Ok(responseWithRoles);
        }

        var user = await _userService.GetByIdAsync(id);

        if (user == null)
        {
            return NotFound(new ApiResponse<object>
            {
                Action = "get",
                HttpStatusCode = (int)HttpStatusCode.NotFound,
                Message = "User not found.",
                Data = null
            });
        }

        var response = new ApiResponse<UserResponse>
        {
            Action = "get",
            HttpStatusCode = (int)HttpStatusCode.OK,
            Message = "User retrieved successfully.",
            Data = MapToResponse(user)
        };

        return Ok(response);
    }



    /// <summary>
    /// Updates an existing user's basic details.
    /// </summary>
    /// <remarks>
    /// Example request body:
    /// <code>
    /// { "username": "johndoe", "fullName": "John Doe", "email": "johndoe@example.com" }
    /// </code>
    /// </remarks>
    /// <param name="id">The unique identifier of the user to update.</param>
    /// <param name="request">Updated user data: <c>username</c>, <c>fullName</c> and <c>email</c>.</param>
    /// <returns>A success or not-found message wrapped in an API response.</returns>
    /// <response code="200">User updated successfully.</response>
    /// <response code="404">User not found.</response>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request)
    {
        var updated = await _userService.UpdateAsync(id, request.Username, request.FullName, request.Email);

        if (!updated)
        {
            return NotFound(new ApiResponse<object>
            {
                Action = "put",
                HttpStatusCode = (int)HttpStatusCode.NotFound,
                Message = "User not found.",
                Data = null
            });
        }

        return Ok(new ApiResponse<object>
        {
            Action = "put",
            HttpStatusCode = (int)HttpStatusCode.OK,
            Message = "User updated successfully.",
            Data = null
        });
    }

    /* [HttpPut("{id:guid}/all-data")]
    public async Task<IActionResult> UpdateAllData(Guid id, [FromBody] UpdateUserAllDataRequest request)
    {
        var updated = await _userService.UpdateAllDataAsync(id, request.Username, request.FullName, request.Email, request.Avatar, request.AvatarBg);

        if (!updated)
        {
            return NotFound(new ApiResponse<object>
            {
                Action = "put",
                HttpStatusCode = (int)HttpStatusCode.NotFound,
                Message = "User not found.",
                Data = null
            });
        }

        return Ok(new ApiResponse<object>
        {
            Action = "put",
            HttpStatusCode = (int)HttpStatusCode.OK,
            Message = "User updated successfully.",
            Data = null
        });
    } */


    /// <summary>
    /// Updates a user's avatar and avatar background color.
    /// </summary>
    /// <remarks>
    /// Example request body:
    /// <code>
    /// { "avatar": "avatar_url", "avatarBg": "#FFFFFF" }
    /// </code>
    /// </remarks>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="request">Avatar data: <c>avatar</c> URL and <c>avatarBg</c> background value.</param>
    /// <returns>A success or not-found message wrapped in an API response.</returns>
    /// <response code="200">User avatar updated successfully.</response>
    /// <response code="404">User not found.</response>
    [HttpPut("{id:guid}/avatar")]
    public async Task<IActionResult> UpdateAvatar(Guid id, [FromBody] UpdateUserAvatarRequest request)
    {
        var updated = await _userService.UpdateAvatarAsync(id, request.Avatar, request.AvatarBg);

        if (!updated)
        {
            return NotFound(new ApiResponse<object>
            {
                Action = "put",
                HttpStatusCode = (int)HttpStatusCode.NotFound,
                Message = "User not found.",
                Data = null
            });
        }

        return Ok(new ApiResponse<object>
        {
            Action = "put",
            HttpStatusCode = (int)HttpStatusCode.OK,
            Message = "User avatar updated successfully.",
            Data = null
        });
    }

    /// <summary>
    /// Updates a user's password.
    /// </summary>
    /// <remarks>
    /// Example request body:
    /// <code>
    /// { "password": "new_secure_password" }
    /// </code>
    /// </remarks>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="request">Password data containing the new <c>password</c>.</param>
    /// <returns>A success or not-found message wrapped in an API response.</returns>
    /// <response code="200">Password updated successfully.</response>
    /// <response code="404">User not found.</response>
    //TODO: Check the security implications of allowing password updates through an API endpoint. Consider implementing additional security measures such as authentication and authorization checks, rate limiting, and logging of password change attempts.
    [HttpPut("{id:guid}/password")]
    public async Task<IActionResult> UpdatePassword(Guid id, [FromBody] UpdateUserPasswordRequest request)
    {
        var updated = await _userService.UpdatePasswordAsync(id, request.Password);

        if (!updated)
        {
            return NotFound(new ApiResponse<object>
            {
                Action = "put",
                HttpStatusCode = (int)HttpStatusCode.NotFound,
                Message = "User not found.",
                Data = null
            });
        }

        return Ok(new ApiResponse<object>
        {
            Action = "put",
            HttpStatusCode = (int)HttpStatusCode.OK,
            Message = "User password updated successfully.",
            Data = null
        });
    }

    /// <summary>
    /// Soft-deletes a user by their ID.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete.</param>
    /// <returns>A success or not-found message wrapped in an API response.</returns>
    /// <response code="200">User deleted successfully.</response>
    /// <response code="404">User not found.</response>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _userService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new ApiResponse<object>
            {
                Action = "delete",
                HttpStatusCode = (int)HttpStatusCode.NotFound,
                Message = "User not found.",
                Data = null
            });
        }

        return Ok(new ApiResponse<object>
        {
            Action = "delete",
            HttpStatusCode = (int)HttpStatusCode.OK,
            Message = "User deleted successfully.",
            Data = null
        });
    }

    /// <summary>
    /// Assigns a single role to a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="roleId">The unique identifier of the role to assign.</param>
    /// <returns>A success message wrapped in an API response.</returns>
    /// <response code="200">Role assigned successfully.</response>
    [HttpPost("{userId:guid}/roles/{roleId:guid}")]
    public async Task<IActionResult> AssignRole(Guid userId, Guid roleId)
    {
        //TODO: test the responses
        /* try
        {
            await _userService.AssignRoleAsync(userId, roleId);
            return Ok(new ApiResponse<object>
            {
                Action = "post",
                HttpStatusCode = (int)HttpStatusCode.OK,
                Message = "Role assigned successfully.",
                Data = null
            });
        }//TODO: In this case, it can be a custom response with a specific error code for role not found, instead of using the generic NotFoundException.
        catch (NotFoundException ex)
        {
            return NotFound(new ApiResponse<object>
            {
                Action = "post",
                HttpStatusCode = (int)HttpStatusCode.NotFound,
                Message = ex.Message,
                Data = null
            });
        } */

        await _userService.AssignRoleAsync(userId, roleId);
        
        return Ok(new ApiResponse<object>
        {
            Action = "post",
            HttpStatusCode = (int)HttpStatusCode.OK,
            Message = "Role assigned successfully.",
            Data = null
        });
    }

    /// <summary>
    /// Assigns multiple roles to a user in a single operation.
    /// </summary>
    /// <remarks>
    /// Example request body:
    /// <code>
    /// [ "roleId1", "roleId2", "roleId3" ]
    /// </code>
    /// </remarks>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="roleIds">A list of role IDs to assign to the user.</param>
    /// <returns>A success message wrapped in an API response.</returns>
    /// <response code="200">Roles assigned successfully.</response>
    [HttpPost("{userId:guid}/roles")]
    public async Task<IActionResult> AssignManyRoles(Guid userId, [FromBody] List<Guid> roleIds)
    {
        await _userService.AssignManyRolesAsync(userId, roleIds);

        return Ok(new ApiResponse<object>
        {
            Action = "post",
            HttpStatusCode = (int)HttpStatusCode.OK,
            Message = "Roles assigned successfully.",
            Data = null
        });
    }

    private static UserResponse MapToResponse(UserDto userDto)
    {
        return new UserResponse
        {
            Id = userDto.Id,
            Username = userDto.Username,
            FullName = userDto.FullName,
            Email = userDto.Email,
            CreatedAt = userDto.CreatedAt,
            IsActive = userDto.IsActive
        };
    }
}