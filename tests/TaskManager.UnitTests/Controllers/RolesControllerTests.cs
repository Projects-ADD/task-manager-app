using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManager.Api.Controllers;
using TaskManager.Application.Features.Permissions.DTOs;
using TaskManager.Application.Features.Roles;
using TaskManager.Application.Features.Roles.DTOs;
using TaskManager.Contracts.Requests;
using TaskManager.Contracts.Responses;

namespace TaskManager.UnitTests.Controllers;

public class RolesControllerTests
{
    private readonly Mock<IRoleService> _roleServiceMock;
    private readonly RolesController _controller;

    public RolesControllerTests()
    {
        _roleServiceMock = new Mock<IRoleService>();
        _controller = new RolesController(_roleServiceMock.Object);
    }

    private static RoleDto CreateRoleDto() => new()
    {
        Id = Guid.NewGuid(),
        Name = "Admin",
        Description = "Administrator role",
        CreatedAt = DateTime.UtcNow,
        IsActive = true
    };

    private static PermissionDto CreatePermissionDto() => new()
    {
        Id = Guid.NewGuid(),
        Name = "tasks.read",
        Description = "Read tasks",
        CreatedAt = DateTime.UtcNow,
        IsActive = true
    };

    [Fact]
    public async Task Create_ShouldReturnCreated_WhenValidRequest()
    {
        var roleDto = CreateRoleDto();
        var request = new CreateRoleRequest { Name = "Admin", Description = "Administrator role" };

        _roleServiceMock.Setup(s => s.CreateAsync(request.Name, request.Description))
            .ReturnsAsync(roleDto);

        var result = await _controller.Create(request);

        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.StatusCode.Should().Be(201);
        createdResult.ActionName.Should().Be(nameof(RolesController.GetById));

        var apiResponse = createdResult.Value.Should().BeOfType<ApiResponse<RoleResponse>>().Subject;
        apiResponse.Action.Should().Be("post");
        apiResponse.HttpStatusCode.Should().Be(201);
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data!.Id.Should().Be(roleDto.Id);
        apiResponse.Data.Name.Should().Be(roleDto.Name);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk_WhenRolesExist()
    {
        var roles = new List<RoleDto> { CreateRoleDto(), CreateRoleDto() };

        _roleServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(roles);

        var result = await _controller.GetAll();
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        var apiResponse = okResult.Value.Should().BeAssignableTo<ApiResponse<List<RoleResponse>>>().Subject;
        apiResponse.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAll_ShouldReturnNotFound_WhenNoRoles()
    {
        _roleServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<RoleDto>());

        var result = await _controller.GetAll();
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.StatusCode.Should().Be(404);
        var apiResponse = notFoundResult.Value.Should().BeAssignableTo<ApiResponse<object>>().Subject;
        apiResponse.Data.Should().BeNull();
    }

    /* [Fact]
    public async Task GetById_ShouldReturnOk_WhenRoleExists()
    {
        var roleDto = CreateRoleDto();

        _roleServiceMock.Setup(s => s.GetByIdAsync(roleDto.Id)).ReturnsAsync(roleDto);

        var result = await _controller.GetById(roleDto.Id);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);

        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<RoleResponse>>().Subject;
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data!.Id.Should().Be(roleDto.Id);
    } */

    /* [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenRoleDoesNotExist()
    {
        var id = Guid.NewGuid();

        _roleServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync((RoleDto?)null);

        var result = await _controller.GetById(id);

        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.StatusCode.Should().Be(404);

        var apiResponse = notFoundResult.Value.Should().BeOfType<ApiResponse<object>>().Subject;
        apiResponse.Data.Should().BeNull();
    } */

    [Fact]
    public async Task Update_ShouldReturnOk_WhenSuccessful()
    {
        var id = Guid.NewGuid();
        var request = new UpdateRoleRequest { Name = "Editor", Description = "Editor role" };

        _roleServiceMock.Setup(s => s.UpdateAsync(id, request.Name, request.Description))
            .ReturnsAsync(true);

        var result = await _controller.Update(id, request);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);

        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<object>>().Subject;
        apiResponse.Action.Should().Be("put");
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenRoleDoesNotExist()
    {
        var id = Guid.NewGuid();
        var request = new UpdateRoleRequest { Name = "Editor", Description = "Editor role" };

        _roleServiceMock.Setup(s => s.UpdateAsync(id, request.Name, request.Description))
            .ReturnsAsync(false);

        var result = await _controller.Update(id, request);

        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.StatusCode.Should().Be(404);

        var apiResponse = notFoundResult.Value.Should().BeOfType<ApiResponse<object>>().Subject;
        apiResponse.Data.Should().BeNull();
    }

    [Fact]
    public async Task Delete_ShouldReturnOk_WhenSuccessful()
    {
        var id = Guid.NewGuid();

        _roleServiceMock.Setup(s => s.DeleteAsync(id)).ReturnsAsync(true);

        var result = await _controller.Delete(id);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);

        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<object>>().Subject;
        apiResponse.Action.Should().Be("delete");
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenRoleDoesNotExist()
    {
        var id = Guid.NewGuid();

        _roleServiceMock.Setup(s => s.DeleteAsync(id)).ReturnsAsync(false);

        var result = await _controller.Delete(id);

        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.StatusCode.Should().Be(404);

        var apiResponse = notFoundResult.Value.Should().BeOfType<ApiResponse<object>>().Subject;
        apiResponse.Data.Should().BeNull();
    }

    [Fact]
    public async Task AssignPermission_ShouldReturnOk_WhenSuccessful()
    {
        var roleId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();

        _roleServiceMock.Setup(s => s.AssignPermissionAsync(roleId, permissionId))
            .Returns(Task.CompletedTask);

        var result = await _controller.AssignPermission(roleId, permissionId);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);

        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<object>>().Subject;
        apiResponse.Action.Should().Be("post");
    }

    [Fact]
    public async Task RevokePermission_ShouldReturnOk_WhenSuccessful()
    {
        var roleId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();

        _roleServiceMock.Setup(s => s.RevokePermissionAsync(roleId, permissionId))
            .Returns(Task.CompletedTask);

        var result = await _controller.RevokePermission(roleId, permissionId);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);

        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<object>>().Subject;
        apiResponse.Action.Should().Be("delete");
    }

    [Fact]
    public async Task GetPermissionsByRole_ShouldReturnOk_WhenRoleHasPermissions()
    {
        var roleId = Guid.NewGuid();
        var permissions = new List<PermissionDto> { CreatePermissionDto(), CreatePermissionDto() };

        _roleServiceMock.Setup(s => s.GetPermissionsByRoleAsync(roleId))
            .ReturnsAsync(permissions);

        var result = await _controller.GetPermissionsByRole(roleId);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);

        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<List<PermissionResponse>>>().Subject;
        apiResponse.Action.Should().Be("get");
        apiResponse.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetPermissionsByRole_ShouldReturnEmptyList_WhenRoleHasNoPermissions()
    {
        var roleId = Guid.NewGuid();

        _roleServiceMock.Setup(s => s.GetPermissionsByRoleAsync(roleId))
            .ReturnsAsync(new List<PermissionDto>());

        var result = await _controller.GetPermissionsByRole(roleId);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);

        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<List<PermissionResponse>>>().Subject;
        apiResponse.Data.Should().BeEmpty();
    }
}
