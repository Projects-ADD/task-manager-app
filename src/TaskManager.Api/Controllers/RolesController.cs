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

public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

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

    //TODO: Test this endpoint with Postman or Swagger.
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


    //TODO: Test this endpoint with Postman or Swagger.
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

    [HttpPost("{roleId}/permissions/{permissionId}")]
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

    //TODO: Test this endpoint with Postman or Swagger.
    [HttpPost("{roleId}/permissions")]
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

    //TODO: Test this endpoint with Postman or Swagger.
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