using DevProject.Application.DTOs.Features;

namespace DevProject.Application.Interfaces;

public interface IFeatureService
{
    Task<FeatureResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<FeatureResponse>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<FeatureResponse> CreateAsync(
        Guid projectId,
        CreateFeatureRequest request,
        CancellationToken cancellationToken = default);
}
