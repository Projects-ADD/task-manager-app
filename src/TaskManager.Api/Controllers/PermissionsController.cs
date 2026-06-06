using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Contracts.Requests;
using TaskManager.Contracts.Responses;
using TaskManager.Domain.Entities;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/permissions")]
public class PermissionsController : ControllerBase
{
    private readonly IPermissionRepository _permissionRepository;

    public PermissionsController(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    [HttpPost]
    public async Task<ActionResult<PermissionResponse>> Create([FromBody] CreatePermissionRequest request)
    {
        var permission = new Permission(request.Name, request.Description);

        await _permissionRepository.AddAsync(permission);
        await _permissionRepository.SaveChangesAsync();

        var response = MapToResponse(permission);

        return CreatedAtAction( nameof(GetById), new { id = permission.Id }, response);
    }

    [HttpGet]
    public async Task<ActionResult<List<PermissionResponse>>> GetAll()
    {
        var permissions = await _permissionRepository.GetAllAsync();

        var response = permissions
            .Select(MapToResponse)
            .ToList();

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PermissionResponse>> GetById(Guid id)
    {
        var permission = await _permissionRepository.GetByIdAsync(id);

        if (permission is null)
        {
            return NotFound();
        }

        return Ok(MapToResponse(permission));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdatePermissionRequest request)
    {
        var permission = await _permissionRepository.GetByIdAsync(id);

        if (permission is null)
        {
            return NotFound();
        }

        permission.Update(
            request.Name,
            request.Description);

        await _permissionRepository.UpdateAsync(permission);
        await _permissionRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var permission = await _permissionRepository.GetByIdAsync(id);

        if (permission is null)
        {
            return NotFound();
        }

        permission.Delete();

        await _permissionRepository.UpdateAsync(permission);
        await _permissionRepository.SaveChangesAsync();

        return NoContent();
    }

    private static PermissionResponse MapToResponse(Permission permission)
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