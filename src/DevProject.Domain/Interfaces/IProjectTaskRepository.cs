using DevProject.Domain.Entities;
using DevProject.Domain.Enums;

namespace DevProject.Domain.Interfaces;

public interface IProjectTaskRepository : IRepository<ProjectTask>
{
    Task<IEnumerable<ProjectTask>> GetFilteredAsync(
        Guid? projectId,
        Guid? featureId,
        string? search,
        ProjectTaskStatus? status,
        TaskPriority? priority,
        CancellationToken cancellationToken = default);
}
