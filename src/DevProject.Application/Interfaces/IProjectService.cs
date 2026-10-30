using DevProject.Application.DTOs.Projects;

namespace DevProject.Application.Interfaces;

public interface IProjectService
{
    Task<ProjectResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<ProjectResponse>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<ProjectResponse> CreateAsync(
        CreateProjectRequest request,
        CancellationToken cancellationToken = default);

    Task<ProjectResponse?> UpdateAsync(
        Guid id,
        UpdateProjectRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}