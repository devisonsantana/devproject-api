using DevProject.Domain.Entities;
using DevProject.Domain.Enums;

namespace DevProject.Domain.Interfaces;

public interface IFeatureRepository : IRepository<Feature>
{
    Task<IEnumerable<Feature>> GetFilteredAsync(
        Guid? projectId,
        string? name,
        FeatureStatus? status,
        CancellationToken cancellationToken = default);
}
