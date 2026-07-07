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


    /*
        POST: api/roles
        Creates a new role with the specified name and description.
        Returns a 201 Created response with the created role's details.

        Body:
        {
            "name": "Role Name",
            "description": "Role Description"
        }

        Example CURL request :
        curl -X POST "https://localhost:5001/api/roles" -H "Content-Type: application/json" -d "{\"name\":\"Admin\",\"description\":\"Administrator role\"}"

        Example response:
        {
            "action": "post",
            "httpStatusCode": 201,
            "message": "Role created successfully.",
            "data": {
                "id": "role-id",
                "name": "Admin",
                "description": "Administrator role",
                "createdAt": "2023-10-01T12:34:56Z",
                "isActive": true
            }
        }
    */
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

    /*
        GET: api/roles
        Retrieves all roles, optionally including their associated permissions.
        Returns a 200 OK response with the list of roles.

        Query Parameters:
        - includePermissions (optional): If true, includes permissions for each role.

        Example CURL request:
        curl -X GET "https://localhost:5001/api/roles?includePermissions=true"

        Example response:
        {
            "action": "get",
            "httpStatusCode": 200,
            "message": "Roles retrieved successfully.",
            "data": [
                {
                    "id": "role-id",
                    "name": "Admin",
                    "description": "Administrator role",
                    "createdAt": "2023-10-01T12:34:56Z",
                    "isActive": true,
                    "permissions": [
                        {
                            "id": "permission-id",
                            "name": "Permission Name",
                            "description": "Permission Description",
                            "createdAt": "2023-10-01T12:34:56Z",
                            "isActive": true
                        }
                    ]
                }
            ]
        }
    */
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


    /*
        GET: api/roles/{id}
        Retrieves a role by its ID, optionally including its associated permissions.
        Returns a 200 OK response with the role's details.

        Path Parameters:
        - id: The unique identifier of the role.

        Query Parameters:
        - includePermissions (optional): If true, includes permissions for the role.

        Example CURL request:
        curl -X GET "https://localhost:5001/api/roles/{id}?includePermissions=true"

        Example response:
        {
            "action": "get",
            "httpStatusCode": 200,
            "message": "Role retrieved successfully.",
            "data": {
                "id": "role-id",
                "name": "Admin",
                "description": "Administrator role",
                "createdAt": "2023-10-01T12:34:56Z",
                "isActive": true,
                "permissions": [
                    {
                        "id": "permission-id",
                        "name": "Permission Name",
                        "description": "Permission Description",
                        "createdAt": "2023-10-01T12:34:56Z",
                        "isActive": true
                    }
                ]
            }
        }
    */
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

    /*
        PUT: api/roles/{id}
        Updates an existing role with the specified name and description.
        Returns a 200 OK response if the update is successful, or a 404 Not Found response if the role does not exist.

        Path Parameters:
        - id: The unique identifier of the role to update.

        Body:
        {
            "name": "Updated Role Name",
            "description": "Updated Role Description"
        }

        Example CURL request:
        curl -X PUT "https://localhost:5001/api/roles/{id}" -H "Content-Type: application/json" -d "{\"name\":\"Updated Admin\",\"description\":\"Updated Administrator role\"}"

        Example response (success):
        {
            "action": "put",
            "httpStatusCode": 200,
            "message": "Role updated successfully.",
            "data": null
        }

        Example response (not found):
        {
            "action": "put",
            "httpStatusCode": 404,
            "message": "Role not found.",
            "data": null
        }
    */
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

    /*
        DELETE: api/roles/{id}
        Deletes an existing role by its ID.
        Returns a 200 OK response if the deletion is successful, or a 404 Not Found response if the role does not exist.

        Path Parameters:
        - id: The unique identifier of the role to delete.

        Example CURL request:
        curl -X DELETE "https://localhost:5001/api/roles/{id}"

        Example response (success):
        {
            "action": "delete",
            "httpStatusCode": 200,
            "message": "Role deleted successfully.",
            "data": null
        }

        Example response (not found):
        {
            "action": "delete",
            "httpStatusCode": 404,
            "message": "Role not found.",
            "data": null
        }
    */
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

    /*
        POST: api/roles/{roleId}/permissions/{permissionId}
        Assigns a permission to a role.
        Returns a 200 OK response if the assignment is successful.

        Path Parameters:
        - roleId: The unique identifier of the role.
        - permissionId: The unique identifier of the permission to assign.

        Example CURL request:
        curl -X POST "https://localhost:5001/api/roles/{roleId}/permissions/{permissionId}"

        Example response:
        {
            "action": "post",
            "httpStatusCode": 200,
            "message": "Permission {permissionId} assigned to role {roleId} successfully.",
            "data": null
        }
    */
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

    /*
        POST: api/roles/{roleId}/permissions
        Assigns multiple permissions to a role.
        Returns a 200 OK response if the assignment is successful.

        Path Parameters:
        - roleId: The unique identifier of the role.

        Body:
        [
            "permission-id-1",
            "permission-id-2",
            "permission-id-3"
        ]

        Example CURL request:
        curl -X POST "https://localhost:5001/api/roles/{roleId}/permissions" -H "Content-Type: application/json" -d "[\"permission-id-1\",\"permission-id-2\"]"

        Example response:
        {
            "action": "post",
            "httpStatusCode": 200,
            "message": "Permissions assigned to role {roleId} successfully.",
            "data": null
        }
    */
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

    /*
        DELETE: api/roles/{roleId}/permissions/{permissionId}
        Revokes a permission from a role.
        Returns a 200 OK response if the revocation is successful.

        Path Parameters:
        - roleId: The unique identifier of the role.
        - permissionId: The unique identifier of the permission to revoke.

        Example CURL request:
        curl -X DELETE "https://localhost:5001/api/roles/{roleId}/permissions/{permissionId}"

        Example response:
        {
            "action": "delete",
            "httpStatusCode": 200,
            "message": "Permission {permissionId} revoked from role {roleId} successfully.",
            "data": null
        }
    */
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

    /*
        DELETE: api/roles/{roleId}/permissions
        Revokes multiple permissions from a role.
        Returns a 200 OK response if the revocation is successful.

        Path Parameters:
        - roleId: The unique identifier of the role.

        Body:
        [
            "permission-id-1",
            "permission-id-2",
            "permission-id-3"
        ]

        Example CURL request:
        curl -X DELETE "https://localhost:5001/api/roles/{roleId}/permissions" -H "Content-Type: application/json" -d "[\"permission-id-1\",\"permission-id-2\"]"

        Example response:
        {
            "action": "delete",
            "httpStatusCode": 200,
            "message": "Permissions revoked from role {roleId} successfully.",
            "data": null
        }
    */
    /*
        TODO: Cuando se revoca 1 permiso asignado y uno no asignado, el endpoint no revoca el permiso asignado y muestra error 409
    */
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

    /*
        GET: api/roles/{roleId}/permissions
        Retrieves all permissions assigned to a specific role.
        Returns a 200 OK response with the list of permissions.

        Path Parameters:
        - roleId: The unique identifier of the role.

        Example CURL request:
        curl -X GET "https://localhost:5001/api/roles/{roleId}/permissions"

        Example response:
        {
            "action": "get",
            "httpStatusCode": 200,
            "message": "Permissions retrieved successfully.",
            "data": [
                {
                    "id": "permission-id-1",
                    "name": "Permission Name 1",
                    "description": "Permission Description 1",
                    "createdAt": "2023-10-01T12:34:56Z",
                    "isActive": true
                },
                {
                    "id": "permission-id-2",
                    "name": "Permission Name 2",
                    "description": "Permission Description 2",
                    "createdAt": "2023-10-01T12:34:56Z",
                    "isActive": true
                }
            ]
        }
    */
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