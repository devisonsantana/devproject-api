using DevProject.Application.DTOs.Tasks;
using DevProject.Application.Interfaces.Mappers;
using DevProject.Domain.Entities;
using DevProject.Domain.Enums;

namespace DevProject.Application.Mappers;

public class ProjectTaskMapper : IProjectTaskMapper
{
    public ProjectTask ToEntity(Guid featureId, CreateProjectTaskRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new(
            featureId,
            request.Title,
            request.Description,
            request.Priority,
            request.DueDate,
            request.Order
        );
    }

    public ProjectTaskResponse ToResponse(ProjectTask task)
    {
        ArgumentNullException.ThrowIfNull(task);

        return new(
            task.Id,
            task.FeatureId,
            task.Title,
            task.Description,
            task.Status,
            task.Priority,
            task.DueDate,
            task.CompletedAt,
            task.Order,
            task.CreatedAt,
            task.UpdatedAt
        );
    }

    public void UpdateEntity(ProjectTask task, UpdateProjectTaskRequest request)
    {
        ArgumentNullException.ThrowIfNull(task);
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException("Task title is required.", nameof(request));
        }

        if (!Enum.IsDefined(request.Priority))
        {
            throw new ArgumentException("Task priority is invalid.", nameof(request));
        }

        task.UpdateDetails(request.Title, request.Description, request.Priority, request.DueDate);
        task.ChangeOrder(request.Order);
    }

    public void UpdateStatus(ProjectTask task, ProjectTaskStatus status)
    {
        ArgumentNullException.ThrowIfNull(task);

        task.ChangeStatus(status);
    }

    public void UpdateOrder(ProjectTask task, int order)
    {
        ArgumentNullException.ThrowIfNull(task);

        task.ChangeOrder(order);
    }
}
