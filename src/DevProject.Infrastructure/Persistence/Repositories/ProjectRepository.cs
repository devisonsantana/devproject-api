using DevProject.Domain.Entities;
using DevProject.Domain.Enums;
using DevProject.Domain.Interfaces;
using DevProject.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace DevProject.Infrastructure.Persistence.Repositories;

public sealed class ProjectRepository : Repository<Project>, IProjectRepository
{
    public ProjectRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Project>> GetFilteredAsync(
        string? name,
        ProjectStatus? status,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Projects.AsNoTracking();
        var normalizedName = name?.Trim();

        if (!string.IsNullOrEmpty(normalizedName))
        {
            query = query.Where(project => EF.Functions.ILike(project.Name, $"%{normalizedName}%"));
        }

        if (status.HasValue)
        {
            query = query.Where(project => project.Status == status.Value);
        }

        return await query
            .OrderBy(project => project.Name)
            .ThenBy(project => project.Id)
            .ToListAsync(cancellationToken);
    }
}
