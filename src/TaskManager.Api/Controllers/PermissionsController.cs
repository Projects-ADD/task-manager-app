using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaskManager.Application.Features.Permissions;
using TaskManager.Application.Features.Permissions.DTOs;
using TaskManager.Contracts.Requests;
using TaskManager.Contracts.Responses;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/permissions")]
[Tags("Permissions")]
public class PermissionsController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    public PermissionsController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    /// <summary>
    /// Creates a new permission.
    /// </summary>
    /// <remarks>
    /// Example request body:
    /// <code>
    /// { "name": "permission_name", "description": "permission_description" }
    /// </code>
    /// </remarks>
    /// <param name="request">Permission data: <c>name</c> and <c>description</c>.</param>
    /// <returns>The newly created permission wrapped in an API response.</returns>
    /// <response code="201">Returns the created permission.</response>
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

    /// <summary>
    /// Retrieves all active permissions.
    /// </summary>
    /// <returns>A list of permissions wrapped in an API response.</returns>
    /// <response code="200">Returns the list of permissions.</response>
    /// <response code="404">No permissions found.</response>
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

    /// <summary>
    /// Retrieves a permission by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the permission.</param>
    /// <param name="showRoles">When <c>true</c>, includes the roles associated with this permission.</param>
    /// <returns>The permission details wrapped in an API response.</returns>
    /// <response code="200">Returns the permission details.</response>
    /// <response code="404">Permission not found.</response>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<PermissionResponse>>> GetById(Guid id, [FromQuery] bool showRoles = false)
    {
        if (showRoles)
        {
            var permissionWithRoles = await _permissionService.GetOneWithRolesAsync(id);

            if (permissionWithRoles is null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Action = "get",
                    HttpStatusCode = (int)HttpStatusCode.NotFound,
                    Message = "Permission not found.",
                    Data = null
                });
            }

            return Ok(new ApiResponse<PermissionWithRolesResponse>
            {
                Action = "get",
                HttpStatusCode = (int)HttpStatusCode.OK,
                Message = "Permission with roles retrieved successfully.",
                Data = new PermissionWithRolesResponse
                {
                    Id = permissionWithRoles.Id,
                    Name = permissionWithRoles.Name,
                    Description = permissionWithRoles.Description,
                    CreatedAt = permissionWithRoles.CreatedAt,
                    IsActive = permissionWithRoles.IsActive,
                    Roles = permissionWithRoles.Roles.Select(r => new RoleResponse
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Description = r.Description,
                        CreatedAt = r.CreatedAt,
                        IsActive = r.IsActive
                    }).ToList()
                }
            });
        }
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

    /// <summary>
    /// Updates an existing permission by its ID.
    /// </summary>
    /// <remarks>
    /// Example request body:
    /// <code>
    /// { "name": "updated_name", "description": "updated_description" }
    /// </code>
    /// </remarks>
    /// <param name="id">The unique identifier of the permission to update.</param>
    /// <param name="request">Updated permission data: <c>name</c> and <c>description</c>.</param>
    /// <returns>A success or not-found message wrapped in an API response.</returns>
    /// <response code="200">Permission updated successfully.</response>
    /// <response code="404">Permission not found.</response>
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

    /// <summary>
    /// Soft-deletes a permission by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the permission to delete.</param>
    /// <returns>A success or not-found message wrapped in an API response.</returns>
    /// <response code="200">Permission deleted successfully.</response>
    /// <response code="404">Permission not found.</response>
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