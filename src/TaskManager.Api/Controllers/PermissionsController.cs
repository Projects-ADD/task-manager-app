using Microsoft.AspNetCore.Mvc;
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


        return CreatedAtAction(
            nameof(GetById),
            new { id = permission.Id },
            new PermissionResponse
            {
                Id = permission.Id,
                Name = permission.Name,
                Description = permission.Description,
                CreatedAt = permission.CreatedAt,
                IsActive = permission.IsActive
            });
    }

    [HttpGet]
    public async Task<ActionResult<List<PermissionResponse>>> GetAll()
    {
        var permissions = await _permissionService.GetAllAsync();

        return Ok(permissions.Select(p => new PermissionResponse
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            CreatedAt = p.CreatedAt,
            IsActive = p.IsActive
        }).ToList());

        //return Ok(permissions.Select(MapToResponse).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PermissionResponse>> GetById(Guid id)
    {
        var permission = await _permissionService.GetByIdAsync(id);

        if (permission is null)
        {
            return NotFound();
        }

        return Ok(new PermissionResponse
        {
            Id = permission.Id,
            Name = permission.Name,
            Description = permission.Description,
            CreatedAt = permission.CreatedAt,
            IsActive = permission.IsActive
        });

        //return Ok(MapToResponse(permission));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdatePermissionRequest request)
    {
        var updated = await _permissionService.UpdateAsync( id, request.Name, request.Description);

        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _permissionService.DeleteAsync(id);

        return deleted ? NoContent() : NotFound();
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