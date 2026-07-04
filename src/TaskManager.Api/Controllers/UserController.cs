using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaskManager.Application.Features.Users;
using TaskManager.Application.Features.Users.DTOs;
using TaskManager.Contracts.Requests;
using TaskManager.Contracts.Responses;

namespace TaskManager.Api.Controllers;


//TODO: Test this controller with Postman or Swagger.
[ApiController]
//[Route("api/[controller]")]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /* 
        POST /api/users
        Creates a new user with the provided details.
        Returns a 201 Created response with the created user's details if successful, or a 400 Bad Request response if the request is invalid.

        Body:
        {
            "username": "string",
            "fullName": "string",
            "email": "string",
            "password": "string"
        }

        Example CURL request:
        curl -X POST "https://localhost:5001/api/users" -H "Content-Type: application/json" -d "{\"username\":\"johndoe\",\"fullName\":\"John Doe\",\"email\":\"johndoe@example.com\",\"password\":\"password123\"}"

        Example response:
        {
            "action": "post",
            "httpStatusCode": 201,
            "message": "User created successfully.",
            "data": {
                "id": "guid",
                "username": "johndoe",
                "fullName": "John Doe",
                "email": "johndoe@example.com"
            }
        }
    */

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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
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