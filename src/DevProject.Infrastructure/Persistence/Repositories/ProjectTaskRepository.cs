using DevProject.Domain.Entities;
using DevProject.Domain.Enums;
using DevProject.Domain.Interfaces;
using DevProject.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace DevProject.Infrastructure.Persistence.Repositories;

public sealed class ProjectTaskRepository : Repository<ProjectTask>, IProjectTaskRepository
{
    public ProjectTaskRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<ProjectTask>> GetFilteredAsync(
        Guid? projectId,
        Guid? featureId,
        string? search,
        ProjectTaskStatus? status,
        TaskPriority? priority,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Tasks.AsNoTracking();
        var normalizedSearch = search?.Trim();

        if (projectId.HasValue)
        {
            query = query.Where(task => Context.Features.Any(feature =>
                feature.Id == task.FeatureId && feature.ProjectId == projectId.Value));
        }

        if (featureId.HasValue)
        {
            query = query.Where(task => task.FeatureId == featureId.Value);
        }

        if (!string.IsNullOrEmpty(normalizedSearch))
        {
            query = query.Where(task =>
                EF.Functions.ILike(task.Title, $"%{normalizedSearch}%")
                || (task.Description != null
                    && EF.Functions.ILike(task.Description, $"%{normalizedSearch}%")));
        }

        if (status.HasValue)
        {
            query = query.Where(task => task.Status == status.Value);
        }

        if (priority.HasValue)
        {
            query = query.Where(task => task.Priority == priority.Value);
        }

        return await query
            .OrderBy(task => task.Order)
            .ThenBy(task => task.Id)
            .ToListAsync(cancellationToken);
    }
}
