using Microsoft.AspNetCore.Mvc;
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

        return CreatedAtAction(
            nameof(GetById),
            new { id = role.Id },
            new RoleResponse
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                CreatedAt = role.CreatedAt,
                IsActive = role.IsActive
            });
    }

    [HttpGet]
    public async Task<ActionResult<List<RoleResponse>>> GetAll()
    {
        var roles = await _roleService.GetAllAsync();

        return Ok(roles.Select(r => new RoleResponse
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            CreatedAt = r.CreatedAt,
            IsActive = r.IsActive
        }).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RoleResponse>> GetById(Guid id)
    {
        var role = await _roleService.GetByIdAsync(id);

        if (role is null)
        {
            return NotFound();
        }

        return Ok(new RoleResponse
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            CreatedAt = role.CreatedAt,
            IsActive = role.IsActive
        });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update( Guid id, [FromBody] UpdateRoleRequest request)
    {
        var updated = await _roleService.UpdateAsync( id, request.Name, request.Description);

        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _roleService.DeleteAsync(id);

        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("{roleId}/permissions/{permissionId}")]
    public async Task<IActionResult> AssignPermission(Guid roleId, Guid permissionId)
    {
        await _roleService.AssignPermissionAsync(roleId, permissionId);

        return NoContent();
    }
}