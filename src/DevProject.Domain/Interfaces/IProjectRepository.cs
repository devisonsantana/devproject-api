using DevProject.Domain.Entities;
using DevProject.Domain.Enums;

namespace DevProject.Domain.Interfaces;

public interface IProjectRepository : IRepository<Project>
{
    Task<IEnumerable<Project>> GetFilteredAsync(
        string? name,
        ProjectStatus? status,
        CancellationToken cancellationToken = default);
}
