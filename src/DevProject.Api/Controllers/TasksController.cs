using DevProject.Application.DTOs.Tasks;
using DevProject.Application.Interfaces;
using DevProject.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DevProject.Api.Controllers;

[ApiController]
[Route("api/tasks")]
public sealed class TasksController(IProjectTaskService taskService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProjectTaskResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProjectTaskResponse>>> Get(
        [FromQuery] Guid? projectId,
        [FromQuery] Guid? featureId,
        [FromQuery] string? search,
        [FromQuery] ProjectTaskStatus? status,
        [FromQuery] TaskPriority? priority,
        CancellationToken cancellationToken)
    {
        var hasFilters = projectId is not null
            || featureId is not null
            || !string.IsNullOrWhiteSpace(search)
            || status is not null
            || priority is not null;
        var tasks = hasFilters
            ? await taskService.SearchAsync(
                new(projectId, featureId, search, status, priority),
                cancellationToken)
            : await taskService.GetAllAsync(cancellationToken);

        return Ok(tasks);
    }

    [HttpGet("{id:guid}", Name = "GetTaskById")]
    [ProducesResponseType(typeof(ProjectTaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectTaskResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var task = await taskService.GetByIdAsync(id, cancellationToken);
        return task is null ? NotFound() : Ok(task);
    }

    [HttpPost("~/api/features/{featureId:guid}/tasks")]
    [ProducesResponseType(typeof(ProjectTaskResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProjectTaskResponse>> Create(
        Guid featureId,
        CreateProjectTaskRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var task = await taskService.CreateAsync(featureId, request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
        }
        catch (ArgumentException exception)
        {
            return ValidationProblem("Invalid task", exception.Message);
        }
    }

    private static ObjectResult ValidationProblem(string title, string detail) =>
        new(new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = title,
            Detail = detail
        })
        {
            StatusCode = StatusCodes.Status400BadRequest,
            ContentTypes = { "application/problem+json" }
        };
}
