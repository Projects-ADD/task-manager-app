using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaskManager.Application.Features.Permissions;
using TaskManager.Application.Features.Permissions.DTOs;
using TaskManager.Contracts.Requests;
using TaskManager.Contracts.Responses;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/permissions")]
public class PermissionsController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    public PermissionsController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    [HttpPost]
    public async Task<ActionResult<PermissionResponse>> Create([FromBody] CreatePermissionRequest request)
    {
        var permission = await _permissionService.CreateAsync( request.Name, request.Description);

        var response = new ApiResponse<PermissionResponse>
        {
            Action = "post",
            HttpStatusCode = (int)HttpStatusCode.Created,
            Message = "Permission created successfully.",
            Data = MapToResponse(permission)
        };

        return CreatedAtAction(
            nameof(GetById),
            new { id = permission.Id },
            response
        );
    }

    [HttpGet]
    public async Task<ActionResult<List<PermissionResponse>>> GetAll()
    {
        var permissions = await _permissionService.GetAllAsync();

        if (permissions == null || !permissions.Any())
        {
            return NotFound(new ApiResponse<object>
            {
                Action = "get",
                HttpStatusCode = (int)HttpStatusCode.NotFound,
                Message = "No permissions found.",
                Data = null
            });
        }

        return Ok(new ApiResponse<List<PermissionResponse>>
        {
            Action = "get",
            HttpStatusCode = (int)HttpStatusCode.OK,
            Message = "Permissions retrieved successfully.",
            Data = permissions.Select(MapToResponse).ToList()
        });

        /* return Ok(permissions.Select(p => new PermissionResponse
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            CreatedAt = p.CreatedAt,
            IsActive = p.IsActive
        }).ToList()); */

        //return Ok(permissions.Select(MapToResponse).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<PermissionResponse>>> GetById(Guid id)
    {
        var permission = await _permissionService.GetByIdAsync(id);

        if (permission is null)
        {
            return NotFound(new ApiResponse<object>
            {
                Action = "get",
                HttpStatusCode = (int)HttpStatusCode.NotFound,
                Message = "Permission not found.",
                Data = null
            });
        }

        return Ok(new ApiResponse<PermissionResponse>
        {
            Action = "get",
            HttpStatusCode = (int)HttpStatusCode.OK,
            Message = "Permission retrieved successfully.",
            Data = MapToResponse(permission)
        });

        //return Ok(MapToResponse(permission));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Update(Guid id, [FromBody] UpdatePermissionRequest request)
    {
        var updated = await _permissionService.UpdateAsync( id, request.Name, request.Description);

        if (!updated)
        {
            return NotFound(new ApiResponse<object>
            {
                Action = "put",
                HttpStatusCode = (int)HttpStatusCode.NotFound,
                Message = "Permission not found.",
                Data = null
            });
        }

        return Ok(new ApiResponse<object>
        {
            Action = "put",
            HttpStatusCode = (int)HttpStatusCode.OK,
            Message = "Permission updated successfully.",
            Data = null
        });
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        var deleted = await _permissionService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new ApiResponse<object>
            {
                Action = "delete",
                HttpStatusCode = (int)HttpStatusCode.NotFound,
                Message = "Permission not found.",
                Data = null
            });
        }


        return Ok(new ApiResponse<object>
        {
            Action = "delete",
            HttpStatusCode = (int)HttpStatusCode.OK,
            Message = "Permission deleted successfully.",
            Data = null
        });
    }

    private static PermissionResponse MapToResponse(PermissionDto permission)
    {
        return new PermissionResponse
        {
            Id = permission.Id,
            Name = permission.Name,
            Description = permission.Description,
            CreatedAt = permission.CreatedAt,
            IsActive = permission.IsActive
        };
    }
}