using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaskManager.Application.Features.Tasks;
using TaskManager.Application.Features.Tasks.DTOs;
using TaskManager.Application.Common.Exceptions;
using TaskManager.Contracts.Requests;
using TaskManager.Contracts.Responses;
using TaskManager.Contracts.Enums;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/tasks")]
[Tags("Tasks")]
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    /// <summary>
    /// Creates a new task.
    /// </summary>
    /// <remarks>
    /// Valid values for <c>status</c>: <c>Draft</c>, <c>Pending</c>, <c>InProgress</c>, <c>Completed</c>, <c>Cancelled</c>.<br/>
    /// Valid values for <c>priority</c>: <c>Low</c>, <c>Medium</c>, <c>High</c>.
    /// <para>
    /// Example request body:
    /// <code>
    /// { "title": "New Task", "description": "Task description", "status": "Draft", "priority": "Medium", "dueAt": "2024-06-01T00:00:00Z" }
    /// </code>
    /// </para>
    /// </remarks>
    /// <param name="request">Task data: <c>title</c>, <c>description</c>, <c>status</c>, <c>priority</c>, and <c>dueAt</c>.</param>
    /// <returns>The newly created task wrapped in an API response.</returns>
    /// <response code="201">Returns the created task.</response>
    /// <response code="400">Invalid request data.</response>
    /// 
     //TODO: The dueDate field is not being saving correctly in the database, it is being saved as bad date. Need to investigate and fix this issue.
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TaskResponse>), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTaskRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<string>
            {
                Action = "post",
                HttpStatusCode = (int)HttpStatusCode.BadRequest,
                Message = "Invalid request data.",
                Data = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
            });
        }

        var taskDto = await _taskService.CreateAsync(
            request.Title, 
            request.Description, 
            MapToDomain(request.Status),
            MapToDomain(request.Priority), 
            request.DueAt
        );

        var response = new ApiResponse<TaskResponse>
        {
            Action = "post",
            HttpStatusCode = (int)HttpStatusCode.Created,
            Message = "Task created successfully.",
            Data = MapToResponse(taskDto)
        };

        return CreatedAtAction(
            nameof(GetTaskById), 
            new { id = taskDto.Id }, 
            response
        );
    }

    /// <summary>
    /// Retrieves all active tasks.
    /// </summary>
    /// <returns>A list of tasks wrapped in an API response.</returns>
    /// <response code="200">Returns the list of tasks.</response>
    /// <response code="404">No tasks found.</response>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var taskDtos = await _taskService.GetAllAsync();

        if (taskDtos == null || !taskDtos.Any())
        {
            return NotFound(new ApiResponse<string>
            {
                Action = "get",
                HttpStatusCode = (int)HttpStatusCode.NotFound,
                Message = "No tasks found.",
                Data = "No tasks available in the system."
            });
        }

        var response = new ApiResponse<IEnumerable<TaskResponse>>
        {
            Action = "get",
            HttpStatusCode = (int)HttpStatusCode.OK,
            Message = "Tasks retrieved successfully.",
            Data = taskDtos.Select(MapToResponse)
        };

        return Ok(response);
    }

    /// <summary>
    /// Retrieves a task by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the task.</param>
    /// <returns>The task details wrapped in an API response.</returns>
    /// <response code="200">Returns the task details.</response>
    /// <response code="404">Task not found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<TaskResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetTaskById(Guid id)
    {
        var taskDto = await _taskService.GetByIdAsync(id);
        if (taskDto == null)
        {
            return NotFound(new ApiResponse<string>
            {
                Action = "get",
                HttpStatusCode = (int)HttpStatusCode.NotFound,
                Message = "Task not found.",
                Data = $"No task found with ID: {id}"
            });
        }

        var response = new ApiResponse<TaskResponse>
        {
            Action = "get",
            HttpStatusCode = (int)HttpStatusCode.OK,
            Message = "Task retrieved successfully.",
            Data = MapToResponse(taskDto)
        };

        return Ok(response);
    }

    /// <summary>
    /// Updates an existing task by its ID.
    /// </summary>
    /// <remarks>
    /// Valid values for <c>status</c>: <c>Draft</c>, <c>Pending</c>, <c>InProgress</c>, <c>Completed</c>, <c>Cancelled</c>.<br/>
    /// Valid values for <c>priority</c>: <c>Low</c>, <c>Medium</c>, <c>High</c>.
    /// <para>
    /// Example request body:
    /// <code>
    /// { "title": "Updated Task", "description": "Updated", "status": "InProgress", "priority": "High", "dueAt": "2024-06-01T00:00:00Z" }
    /// </code>
    /// </para>
    /// </remarks>
    /// <param name="id">The unique identifier of the task to update.</param>
    /// <param name="request">Updated task data.</param>
    /// <returns>A success, not-found or bad-request message wrapped in an API response.</returns>
    /// <response code="200">Task updated successfully.</response>
    /// <response code="404">Task not found.</response>
    /// <response code="400">Invalid request data.</response>
    //TODO: The dueDate field is not being saving correctly in the database, it is being saved as bad date. Need to investigate and fix this issue.
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<string>
            {
                Action = "put",
                HttpStatusCode = (int)HttpStatusCode.BadRequest,
                Message = "Invalid request data.",
                Data = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
            });
        }

        bool updated = await _taskService.UpdateAsync(
            id,
            request.Title,
            request.Description,
            MapToDomain(request.Status),
            MapToDomain(request.Priority),
            request.DueAt
        );

        if (!updated)
        {
            return NotFound(new ApiResponse<string>
            {
                Action = "put",
                HttpStatusCode = (int)HttpStatusCode.NotFound,
                Message = "Task not found.",
                Data = $"No task found with ID: {id}"
            });
        }

        var response = new ApiResponse<string>
        {
            Action = "put",
            HttpStatusCode = (int)HttpStatusCode.OK,
            Message = "Task updated successfully.",
            Data = $"Task with ID: {id} updated successfully."
        };

        return Ok(response);
    }


    /// <summary>
    /// Soft-deletes a task by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the task to delete.</param>
    /// <returns>A success or not-found message wrapped in an API response.</returns>
    /// <response code="200">Task deleted successfully.</response>
    /// <response code="404">Task not found.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Delete(Guid id)
    {
        bool deleted = await _taskService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new ApiResponse<string>
            {
                Action = "delete",
                HttpStatusCode = (int)HttpStatusCode.NotFound,
                Message = "Task not found.",
                Data = $"No task found with ID: {id}"
            });
        }

        var response = new ApiResponse<string>
        {
            Action = "delete",
            HttpStatusCode = (int)HttpStatusCode.OK,
            Message = "Task deleted successfully.",
            Data = $"Task with ID: {id} deleted successfully."
        };

        return Ok(response);
    }

    //MapToResponse method to map TaskDto to TaskResponse
    private static TaskResponse MapToResponse(TaskDto taskDto) =>
        new TaskResponse
        {
            Id = taskDto.Id,
            Title = taskDto.Title,
            Description = taskDto.Description,
            Status = MapToContract(taskDto.Status),
            Priority = MapToContract(taskDto.Priority),
            DueAt = taskDto.DueAt,
            CreatedAt = taskDto.CreatedAt
        };



    private static Domain.Enums.TaskStatus MapToDomain(TaskStatusContract status) =>
    status switch
    {
        TaskStatusContract.Draft => Domain.Enums.TaskStatus.Draft,
        TaskStatusContract.Pending => Domain.Enums.TaskStatus.Pending,
        TaskStatusContract.InProgress => Domain.Enums.TaskStatus.InProgress,
        TaskStatusContract.Completed => Domain.Enums.TaskStatus.Completed,
        TaskStatusContract.Cancelled => Domain.Enums.TaskStatus.Cancelled,
        _ => throw new BusinessRuleException("Invalid task status.")
    };

    private static Domain.Enums.TaskPriority MapToDomain(TaskPriorityContract priority) =>
    priority switch
    {
        TaskPriorityContract.Low => Domain.Enums.TaskPriority.Low,
        TaskPriorityContract.Medium => Domain.Enums.TaskPriority.Medium,
        TaskPriorityContract.High => Domain.Enums.TaskPriority.High,
        _ => throw new BusinessRuleException("Invalid task priority.")
    };

    private static TaskStatusContract MapToContract(Domain.Enums.TaskStatus status) =>
    status switch
    {
        Domain.Enums.TaskStatus.Draft => TaskStatusContract.Draft,
        Domain.Enums.TaskStatus.Pending => TaskStatusContract.Pending,
        Domain.Enums.TaskStatus.InProgress => TaskStatusContract.InProgress,
        Domain.Enums.TaskStatus.Completed => TaskStatusContract.Completed,
        Domain.Enums.TaskStatus.Cancelled => TaskStatusContract.Cancelled,
        _ => throw new BusinessRuleException("Invalid task status.")
    };

    private static TaskPriorityContract MapToContract(Domain.Enums.TaskPriority priority) =>
    priority switch
    {
        Domain.Enums.TaskPriority.Low => TaskPriorityContract.Low,
        Domain.Enums.TaskPriority.Medium => TaskPriorityContract.Medium,
        Domain.Enums.TaskPriority.High => TaskPriorityContract.High,
        _ => throw new BusinessRuleException("Invalid task priority.")
    };
}
    