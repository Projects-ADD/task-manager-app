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
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    /*
     * POST: api/tasks
     * Creates a new task with the provided details.
     * Returns a 201 Created response with the created task's details if successful, or a 400 Bad Request response if the request is invalid.
     *
     * Body:
     * {
     *     "title": "string",
     *     "description": "string",
     *     "dueDate": "2024-06-01T00:00:00Z",
     *     "status": "string", ["Draft", "Pending", "InProgress", "Completed", "Cancelled"]
     *     "priority": "string" ["Low", "Medium", "High"]
     * }
     *
     * Example CURL request:
     * curl -X POST "https://localhost:5001/api/tasks" -H "Content-Type: application/json" -d "{\"title\":\"Task 1\",\"description\":\"Description 1\",\"dueDate\":\"2024-06-01T00:00:00Z\",\"status\":\"InProgress\",\"priority\":\"High\"}"
     *
     * Example response:
     * {
     *     "action": "post",
     *     "httpStatusCode": 201,
     *     "message": "Task created successfully.",
     *     "data": {
     *         "id": "guid",
     *         "title": "Task 1",
     *         "description": "Description 1",
     *         "dueDate": "2024-06-01T00:00:00Z",
     *         "status": "InProgress",
     *         "priority": "High"
     *     }
     * }
     */
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

    /*
     * GET: api/tasks
     * Retrieves all tasks in the system.
     * Returns a 200 OK response with a list of tasks if found, or a 404 Not Found response if no tasks exist.
     *
     * Example CURL request:
     * curl -X GET "https://localhost:5001/api/tasks"
     *
     * Example response (200 OK):
     * {
     *     "action": "get",
     *     "httpStatusCode": 200,
     *     "message": "Tasks retrieved successfully.",
     *     "data": [
     *         {
     *             "id": "guid",
     *             "title": "Task 1",
     *             "description": "Description 1",
     *             "dueDate": "2024-06-01T00:00:00Z",
     *             "status": "InProgress",
     *             "priority": "High"
     *         },
     *         {
     *             "id": "guid",
     *             "title": "Task 2",
     *             "description": "Description 2",
     *             "dueDate": "2024-06-02T00:00:00Z",
     *             "status": "Pending",
     *             "priority": "Medium"
     *         }
     *     ]
     * }
     *
     * Example response (404 Not Found):
     * {
     *     "action": "get",
     *     "httpStatusCode": 404,
     *     "message": "No tasks found.",
     *     "data": "No tasks available in the system."
     * }
    */
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

    /*
    * GET: api/tasks/{id}
    * Retrieves a task by its unique identifier.
    * Returns a 200 OK response with the task's details if found, or a 404 Not Found response if the task does not exist.
    *
    * Example CURL request:
    * curl -X GET "https://localhost:5001/api/tasks/{id}"
    *
    * Example response (200 OK):
    * {
    *     "action": "get",
    *     "httpStatusCode": 200,
    *     "message": "Task retrieved successfully.",
    *     "data": {
    *         "id": "guid",
    *         "title": "Task 1",
    *         "description": "Description 1",
    *         "dueDate": "2024-06-01T00:00:00Z",
    *         "status": "InProgress",
    *         "priority": "High"
    *     }
    * }
    *
    * Example response (404 Not Found):
    * {
    *     "action": "get",
    *     "httpStatusCode": 404,
    *     "message": "Task not found.",
    *     "data": "No task found with ID: {id}"
    * }
    */
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

    /*
    * PUT: api/tasks/{id}
    * Updates an existing task with the provided details.
    * Returns a 200 OK response with a success message if the update is successful, a 404 Not Found response if the task does not exist, or a 400 Bad Request response if the request is invalid.
    *
    * Example request body:
    * {
    *     "title": "Updated Task",
    *     "description": "Updated Description",
    *     "status": "InProgress", ["Draft", "Pending", "InProgress", "Completed", "Cancelled"]
    *     "priority": "High" ["Low", "Medium", "High"]
    *     "dueAt": "2024-06-01T00:00:00Z"
    * }
    *
    * Example CURL request:
    * curl -X PUT "https://localhost:5001/api/tasks/{id}" -H "Content-Type: application/json" -d '{"title":"Updated Task","description":"Updated Description","status":"InProgress","priority":"High","dueAt":"2024-06-01T00:00:00Z"}'
    *
    * Example response (200 OK):
    * {
    *     "action": "put",
    *     "httpStatusCode": 200,
    *     "message": "Task updated successfully.",
    *     "data": "Task with ID: {id} updated successfully."
    * }
    *
    * Example response (404 Not Found):
    * {
    *     "action": "put",
    *     "httpStatusCode": 404,
    *     "message": "Task not found.",
    *     "data": "No task found with ID: {id}"
    * }
    *
    * Example response (400 Bad Request):
    * {
    *     "action": "put",
    *     "httpStatusCode": 400,
    *     "message": "Invalid request data.",
    *     "data": "Error details..."
    * }
    */
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


    /*
    * DELETE: api/tasks/{id}
    * Deletes a task by its unique identifier.
    * Returns a 200 OK response with a success message if the deletion is successful, or a 404 Not Found response if the task does not exist.
    * 
    * Example CURL request:
    * curl -X DELETE "https://localhost:5001/api/tasks/{id}"
    * 
    * Example response (200 OK):
    * {
    *     "action": "delete",
    *     "httpStatusCode": 200,
    *     "message": "Task deleted successfully.",
    *     "data": "Task with ID: {id} deleted successfully."
    * }
    */
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
    