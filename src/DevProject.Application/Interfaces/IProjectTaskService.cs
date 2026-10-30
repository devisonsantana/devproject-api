using DevProject.Application.DTOs.Tasks;
using DevProject.Application.Queries.Tasks;

namespace DevProject.Application.Interfaces;

public interface IProjectTaskService
{
    Task<ProjectTaskResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<ProjectTaskResponse>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<IEnumerable<ProjectTaskResponse>> SearchAsync(
        ProjectTaskSearchQuery query,
        CancellationToken cancellationToken = default);

    Task<ProjectTaskResponse> CreateAsync(
        Guid featureId,
        CreateProjectTaskRequest request,
        CancellationToken cancellationToken = default);
}
