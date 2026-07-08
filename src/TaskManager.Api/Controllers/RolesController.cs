using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaskManager.Application.Features.Permissions.DTOs;
using TaskManager.Application.Features.Roles;
using TaskManager.Application.Features.Roles.DTOs;
using TaskManager.Contracts.Requests;
using TaskManager.Contracts.Responses;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/roles")]
[Tags("Roles")]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }


    /// <summary>
    /// Creates a new role.
    /// </summary>
    /// <remarks>
    /// Example request body:
    /// <code>
    /// { "name": "Admin", "description": "Administrator role" }
    /// </code>
    /// </remarks>
    /// <param name="request">Role data: <c>name</c> and <c>description</c>.</param>
    /// <returns>The newly created role wrapped in an API response.</returns>
    /// <response code="201">Returns the created role.</response>
    [HttpPost]
    public async Task<ActionResult<RoleResponse>> Create([FromBody] CreateRoleRequest request)
    {
        var role = await _roleService.CreateAsync(request.Name, request.Description);

        var response = new ApiResponse<RoleResponse>
        {
            Action = "post",
            HttpStatusCode = (int)HttpStatusCode.Created,
            Message = "Role created successfully.",
            Data = MapToResponse(role)
        };

        return CreatedAtAction(
            nameof(GetById),
            new { id = role.Id },
            response
        );

    }

    /// <summary>
    /// Retrieves all active roles, optionally including their associated permissions.
    /// </summary>
    /// <param name="includePermissions">When <c>true</c>, includes the permissions assigned to each role.</param>
    /// <returns>A list of roles wrapped in an API response.</returns>
    /// <response code="200">Returns the list of roles.</response>
    /// <response code="404">No roles found.</response>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool includePermissions = false)
    {
        if (includePermissions)
        {
            var rolesWithPermissions = await _roleService.GetAllWithPermissionsAsync();

            if (rolesWithPermissions == null || !rolesWithPermissions.Any())
            {
                return NotFound(new ApiResponse<object>
                {
                    Action = "get",
                    HttpStatusCode = (int)HttpStatusCode.NotFound,
                    Message = "No roles found.",
                    Data = null
                });
            }

            return Ok(new ApiResponse<List<RoleWithPermissionsResponse>>
            {
                Action = "get",
                HttpStatusCode = (int)HttpStatusCode.OK,
                Message = "Roles retrieved successfully.",
                Data = rolesWithPermissions.Select(MapToWithPermissionsResponse).ToList()
            });
        }

        var roles = await _roleService.GetAllAsync();

        if (roles == null || !roles.Any())
        {
            return NotFound(new ApiResponse<object>
            {
                Action = "get",
                HttpStatusCode = (int)HttpStatusCode.NotFound,
                Message = "No roles found.",
                Data = null
            });
        }

        return Ok(new ApiResponse<List<RoleResponse>>
        {
            Action = "get",
            HttpStatusCode = (int)HttpStatusCode.OK,
            Message = "Roles retrieved successfully.",
            Data = roles.Select(MapToResponse).ToList()
        });
    }


    /// <summary>
    /// Retrieves a role by its unique identifier, optionally including its associated permissions.
    /// </summary>
    /// <param name="id">The unique identifier of the role.</param>
    /// <param name="includePermissions">When <c>true</c>, includes the permissions assigned to the role.</param>
    /// <returns>The role details wrapped in an API response.</returns>
    /// <response code="200">Returns the role details.</response>
    /// <response code="404">Role not found.</response>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, [FromQuery] bool includePermissions = false)
    {
        if (includePermissions)
        {
            var roleWithPermissions = await _roleService.GetByIdWithPermissionsAsync(id);

            if (roleWithPermissions is null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Action = "get",
                    HttpStatusCode = (int)HttpStatusCode.NotFound,
                    Message = "Role not found.",
                    Data = null
                });
            }

            return Ok(new ApiResponse<RoleWithPermissionsResponse>
            {
                Action = "get",
                HttpStatusCode = (int)HttpStatusCode.OK,
                Message = "Role retrieved successfully.",
                Data = MapToWithPermissionsResponse(roleWithPermissions)
            });
        }

        var role = await _roleService.GetByIdAsync(id);

        if (role is null)
        {
            return NotFound(new ApiResponse<object>
            {
                Action = "get",
                HttpStatusCode = (int)HttpStatusCode.NotFound,
                Message = "Role not found.",
                Data = null
            });
        }

        return Ok(new ApiResponse<RoleResponse>
        {
            Action = "get",
            HttpStatusCode = (int)HttpStatusCode.OK,
            Message = "Role retrieved successfully.",
            Data = MapToResponse(role)
        });
    }

    /// <summary>
    /// Updates an existing role by its ID.
    /// </summary>
    /// <remarks>
    /// Example request body:
    /// <code>
    /// { "name": "Updated Admin", "description": "Updated Administrator role" }
    /// </code>
    /// </remarks>
    /// <param name="id">The unique identifier of the role to update.</param>
    /// <param name="request">Updated role data: <c>name</c> and <c>description</c>.</param>
    /// <returns>A success or not-found message wrapped in an API response.</returns>
    /// <response code="200">Role updated successfully.</response>
    /// <response code="404">Role not found.</response>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update( Guid id, [FromBody] UpdateRoleRequest request)
    {
        var updated = await _roleService.UpdateAsync( id, request.Name, request.Description);

        if (!updated)
        {
            return NotFound(new ApiResponse<object>
            {
                Action = "put",
                HttpStatusCode = (int)HttpStatusCode.NotFound,
                Message = "Role not found.",
                Data = null
            });
        }

        return Ok(new ApiResponse<object>
        {
            Action = "put",
            HttpStatusCode = (int)HttpStatusCode.OK,
            Message = "Role updated successfully.",
            Data = null
        });
    }

    /// <summary>
    /// Soft-deletes a role by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the role to delete.</param>
    /// <returns>A success or not-found message wrapped in an API response.</returns>
    /// <response code="200">Role deleted successfully.</response>
    /// <response code="404">Role not found.</response>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        //TODO: This function should return the role that was deleted.
        var deleted = await _roleService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new ApiResponse<object>
            {
                Action = "delete",
                HttpStatusCode = (int)HttpStatusCode.NotFound,
                Message = "Role not found.",
                Data = null
            });
        }

        return Ok(new ApiResponse<object>
        {
            Action = "delete",
            HttpStatusCode = (int)HttpStatusCode.OK,
            Message = "Role deleted successfully.",
            Data = null
        });
    }

    /// <summary>
    /// Assigns a single permission to a role.
    /// </summary>
    /// <param name="roleId">The unique identifier of the role.</param>
    /// <param name="permissionId">The unique identifier of the permission to assign.</param>
    /// <returns>A success message wrapped in an API response.</returns>
    /// <response code="200">Permission assigned successfully.</response>
    [HttpPost("{roleId:guid}/permissions/{permissionId:guid}")]
    public async Task<IActionResult> AssignPermission(Guid roleId, Guid permissionId)
    {
        await _roleService.AssignPermissionAsync(roleId, permissionId);

        return Ok(new ApiResponse<object>
        {
            Action = "post",
            HttpStatusCode = (int)HttpStatusCode.OK,
            Message = $"Permission {permissionId} assigned to role {roleId} successfully.",
            Data = null
        });
    }

    /// <summary>
    /// Assigns multiple permissions to a role in a single operation.
    /// </summary>
    /// <remarks>
    /// Example request body:
    /// <code>
    /// [ "permission-id-1", "permission-id-2", "permission-id-3" ]
    /// </code>
    /// </remarks>
    /// <param name="roleId">The unique identifier of the role.</param>
    /// <param name="permissionIds">A list of permission IDs to assign to the role.</param>
    /// <returns>A success message wrapped in an API response.</returns>
    /// <response code="200">Permissions assigned successfully.</response>
    [HttpPost("{roleId:guid}/permissions")]
    public async Task<IActionResult> AssignManyPermissions(Guid roleId, [FromBody] List<Guid> permissionIds)
    {
        await _roleService.AssignManyPermissionsAsync(roleId, permissionIds);

        return Ok(new ApiResponse<object>
        {
            Action = "post",
            HttpStatusCode = (int)HttpStatusCode.OK,
            Message = $"Permissions assigned to role {roleId} successfully.",
            Data = null
        });
    }

    /// <summary>
    /// Assigns multiple users to a role in a single operation.
    /// </summary>
    /// <remarks>
    /// Example request body:
    /// <code>
    /// [ "user-id-1", "user-id-2", "user-id-3" ]
    /// </code>
    /// </remarks>
    /// <param name="roleId">The unique identifier of the role.</param>
    /// <param name="userIds">A list of user IDs to assign to the role.</param>
    /// <returns>A success message wrapped in an API response.</returns>
    /// <response code="200">Users assigned successfully.</response>
    [HttpPost("{roleId:guid}/users")]
    public async Task<IActionResult> AssignManyUsers(Guid roleId, [FromBody] List<Guid> userIds)
    {
        await _roleService.AssignManyUsersAsync(roleId, userIds);

        return Ok(new ApiResponse<object>
        {
            Action = "post",
            HttpStatusCode = (int)HttpStatusCode.OK,
            Message = $"Users assigned to role {roleId} successfully.",
            Data = null
        });
    }

    /// <summary>
    /// Revokes a single permission from a role.
    /// </summary>
    /// <param name="roleId">The unique identifier of the role.</param>
    /// <param name="permissionId">The unique identifier of the permission to revoke.</param>
    /// <returns>A success message wrapped in an API response.</returns>
    /// <response code="200">Permission revoked successfully.</response>
    [HttpDelete("{roleId}/permissions/{permissionId}")]
    public async Task<IActionResult> RevokePermission(Guid roleId, Guid permissionId)
    {
        await _roleService.RevokePermissionAsync(roleId, permissionId);

        return Ok(new ApiResponse<object>
        {
            Action = "delete",
            HttpStatusCode = (int)HttpStatusCode.OK,
            Message = $"Permission {permissionId} revoked from role {roleId} successfully.",
            Data = null
        });
    }

    /// <summary>
    /// Revokes multiple permissions from a role in a single operation.
    /// </summary>
    /// <remarks>
    /// Example request body:
    /// <code>
    /// [ "permission-id-1", "permission-id-2", "permission-id-3" ]
    /// </code>
    /// <para>
    /// <strong>Known issue:</strong> When revoking a mix of assigned and unassigned permissions,
    /// the endpoint currently returns a 409 error without processing any revocations.
    /// </para>
    /// </remarks>
    /// <param name="roleId">The unique identifier of the role.</param>
    /// <param name="permissionIds">A list of permission IDs to revoke from the role.</param>
    /// <returns>A success message wrapped in an API response.</returns>
    /// <response code="200">Permissions revoked successfully.</response>
    [HttpDelete("{roleId}/permissions")]
    public async Task<IActionResult> RevokeManyPermissions(Guid roleId, [FromBody] List<Guid> permissionIds)
    {
        await _roleService.RevokeManyPermissionsAsync(roleId, permissionIds);

        return Ok(new ApiResponse<object>
        {
            Action = "delete",
            HttpStatusCode = (int)HttpStatusCode.OK,
            Message = $"Permissions revoked from role {roleId} successfully.",
            Data = null
        });
    }

    /// <summary>
    /// Retrieves all permissions assigned to a specific role.
    /// </summary>
    /// <param name="roleId">The unique identifier of the role.</param>
    /// <returns>A list of permissions assigned to the role, wrapped in an API response.</returns>
    /// <response code="200">Returns the list of permissions assigned to the role.</response>
    [HttpGet("{roleId}/permissions")]
    public async Task<ActionResult<ApiResponse<List<PermissionResponse>>>> GetPermissionsByRole(Guid roleId)
    {
        var permissions = await _roleService.GetPermissionsByRoleAsync(roleId);

        return Ok(new ApiResponse<List<PermissionResponse>>
        {
            Action = "get",
            HttpStatusCode = (int)HttpStatusCode.OK,
            Message = "Permissions retrieved successfully.",
            Data = permissions.Select(MapToPermissionResponse).ToList()
        });
    }

    private static RoleResponse MapToResponse(RoleDto role)
    {
        return new RoleResponse
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            CreatedAt = role.CreatedAt,
            IsActive = role.IsActive
        };
    }

    private static PermissionResponse MapToPermissionResponse(PermissionDto permission)
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

    private static RoleWithPermissionsResponse MapToWithPermissionsResponse(RoleWithPermissionsDto role)
    {
        return new RoleWithPermissionsResponse
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            CreatedAt = role.CreatedAt,
            IsActive = role.IsActive,
            Permissions = role.Permissions.Select(MapToPermissionResponse).ToList()
        };
    }
}