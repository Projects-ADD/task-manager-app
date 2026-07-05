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

    /*
        POST /api/permissions
        Creates a new permission.
        Returns a 201 Created response if the creation is successful.

        Body:
        {
            "name": "string",
            "description": "string"
        }

        Example CURL request:
        curl -X POST "https://localhost:5001/api/permissions" -H "Content-Type: application/json" -d "{\"name\":\"permission_name\",\"description\":\"permission_description\"}"

        Example response:
        {
            "action": "post",
            "httpStatusCode": 201,
            "message": "Permission created successfully.",
            "data": {
                "id": "guid",
                "name": "string",
                "description": "string",
                "createdAt": "2024-06-01T00:00:00Z",
                "isActive": true
            }
        }
    */
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

    /*
        GET /api/permissions
        Retrieves all permissions.
        Returns a 200 OK response with the list of permissions.

        Example CURL request:
        curl -X GET "https://localhost:5001/api/permissions"
        
        Example response:
        {
            "action": "get",
            "httpStatusCode": 200,
            "message": "Permissions retrieved successfully.",
            "data": [
                {
                    "id": "guid",
                    "name": "string",
                    "description": "string",
                    "createdAt": "2024-06-01T00:00:00Z",
                    "isActive": true
                }
            ]
        }
    */
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

    /*
        GET /api/permissions/{id}
        Retrieves a permission by its ID.
        Returns a 200 OK response with the permission details if found, or a 404 Not Found response if not found.

        Example CURL request:
        curl -X GET "https://localhost:5001/api/permissions/{id}"
        
        Example response (if found):
        {
            "action": "get",
            "httpStatusCode": 200,
            "message": "Permission retrieved successfully.",
            "data": {
                "id": "guid",
                "name": "string",
                "description": "string",
                "createdAt": "2024-06-01T00:00:00Z",
                "isActive": true
            }
        }

        Example response (if not found):
        {
            "action": "get",
            "httpStatusCode": 404,
            "message": "Permission not found.",
            "data": null
        }
    */
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

    /*
        PUT /api/permissions/{id}
        Updates a permission by its ID.
        Returns a 200 OK response if the update is successful, or a 404 Not Found response if the permission is not found.

        Body:
        {
            "name": "string",
            "description": "string"
        }

        Example CURL request:
        curl -X PUT "https://localhost:5001/api/permissions/{id}" -H "Content-Type: application/json" -d "{\"name\":\"updated_name\",\"description\":\"updated_description\"}"

        Example response (if updated):
        {
            "action": "put",
            "httpStatusCode": 200,
            "message": "Permission updated successfully.",
            "data": null
        }

        Example response (if not found):
        {
            "action": "put",
            "httpStatusCode": 404,
            "message": "Permission not found.",
            "data": null
        }
    */
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

    /*
        DELETE /api/permissions/{id}
        Deletes a permission by its ID.
        Returns a 200 OK response if the deletion is successful, or a 404 Not Found response if the permission is not found.

        Example CURL request:
        curl -X DELETE "https://localhost:5001/api/permissions/{id}"

        Example response (if deleted):
        {
            "action": "delete",
            "httpStatusCode": 200,
            "message": "Permission deleted successfully.",
            "data": null
        }

        Example response (if not found):
        {
            "action": "delete",
            "httpStatusCode": 404,
            "message": "Permission not found.",
            "data": null
        }
    */
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