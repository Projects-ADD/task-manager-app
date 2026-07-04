using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManager.Api.Controllers;
using TaskManager.Application.Features.Permissions;
using TaskManager.Application.Features.Permissions.DTOs;
using TaskManager.Contracts.Requests;
using TaskManager.Contracts.Responses;

namespace TaskManager.UnitTests.Controllers;

public class PermissionsControllerTests
{
    private readonly Mock<IPermissionService> _permissionServiceMock;
    private readonly PermissionsController _controller;

    public PermissionsControllerTests()
    {
        _permissionServiceMock = new Mock<IPermissionService>();
        _controller = new PermissionsController(_permissionServiceMock.Object);
    }

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
        var permissionDto = CreatePermissionDto();
        var request = new CreatePermissionRequest { Name = "tasks.read", Description = "Read tasks" };

        _permissionServiceMock.Setup(s => s.CreateAsync(request.Name, request.Description))
            .ReturnsAsync(permissionDto);

        var result = await _controller.Create(request);

        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.StatusCode.Should().Be(201);
        createdResult.ActionName.Should().Be(nameof(PermissionsController.GetById));

        var apiResponse = createdResult.Value.Should().BeOfType<ApiResponse<PermissionResponse>>().Subject;
        apiResponse.Action.Should().Be("post");
        apiResponse.HttpStatusCode.Should().Be(201);
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data!.Id.Should().Be(permissionDto.Id);
        apiResponse.Data.Name.Should().Be(permissionDto.Name);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk_WhenPermissionsExist()
    {
        var permissions = new List<PermissionDto> { CreatePermissionDto(), CreatePermissionDto() };

        _permissionServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(permissions);

        var result = await _controller.GetAll();

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);

        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<List<PermissionResponse>>>().Subject;
        apiResponse.Action.Should().Be("get");
        apiResponse.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAll_ShouldReturnNotFound_WhenNoPermissions()
    {
        _permissionServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<PermissionDto>());

        var result = await _controller.GetAll();

        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.StatusCode.Should().Be(404);

        var apiResponse = notFoundResult.Value.Should().BeOfType<ApiResponse<object>>().Subject;
        apiResponse.Action.Should().Be("get");
        apiResponse.Data.Should().BeNull();
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenPermissionExists()
    {
        var permissionDto = CreatePermissionDto();

        _permissionServiceMock.Setup(s => s.GetByIdAsync(permissionDto.Id)).ReturnsAsync(permissionDto);

        var result = await _controller.GetById(permissionDto.Id);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);

        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<PermissionResponse>>().Subject;
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data!.Id.Should().Be(permissionDto.Id);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenPermissionDoesNotExist()
    {
        var id = Guid.NewGuid();

        _permissionServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync((PermissionDto?)null);

        var result = await _controller.GetById(id);

        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.StatusCode.Should().Be(404);

        var apiResponse = notFoundResult.Value.Should().BeOfType<ApiResponse<object>>().Subject;
        apiResponse.Data.Should().BeNull();
    }

    [Fact]
    public async Task Update_ShouldReturnOk_WhenSuccessful()
    {
        var id = Guid.NewGuid();
        var request = new UpdatePermissionRequest { Name = "tasks.write", Description = "Write tasks" };

        _permissionServiceMock.Setup(s => s.UpdateAsync(id, request.Name, request.Description))
            .ReturnsAsync(true);

        var result = await _controller.Update(id, request);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);

        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<object>>().Subject;
        apiResponse.Action.Should().Be("put");
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenPermissionDoesNotExist()
    {
        var id = Guid.NewGuid();
        var request = new UpdatePermissionRequest { Name = "tasks.write", Description = "Write tasks" };

        _permissionServiceMock.Setup(s => s.UpdateAsync(id, request.Name, request.Description))
            .ReturnsAsync(false);

        var result = await _controller.Update(id, request);

        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.StatusCode.Should().Be(404);

        var apiResponse = notFoundResult.Value.Should().BeOfType<ApiResponse<object>>().Subject;
        apiResponse.Data.Should().BeNull();
    }

    [Fact]
    public async Task Delete_ShouldReturnOk_WhenSuccessful()
    {
        var id = Guid.NewGuid();

        _permissionServiceMock.Setup(s => s.DeleteAsync(id)).ReturnsAsync(true);

        var result = await _controller.Delete(id);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);

        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<object>>().Subject;
        apiResponse.Action.Should().Be("delete");
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenPermissionDoesNotExist()
    {
        var id = Guid.NewGuid();

        _permissionServiceMock.Setup(s => s.DeleteAsync(id)).ReturnsAsync(false);

        var result = await _controller.Delete(id);

        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.StatusCode.Should().Be(404);

        var apiResponse = notFoundResult.Value.Should().BeOfType<ApiResponse<object>>().Subject;
        apiResponse.Data.Should().BeNull();
    }
}
