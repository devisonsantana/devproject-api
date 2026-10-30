using DevProject.Domain.Entities;
using DevProject.Domain.Enums;
using DevProject.Domain.Interfaces;
using DevProject.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace DevProject.Infrastructure.Persistence.Repositories;

public sealed class FeatureRepository : Repository<Feature>, IFeatureRepository
{
    public FeatureRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Feature>> GetFilteredAsync(
        Guid? projectId,
        string? name,
        FeatureStatus? status,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Features.AsNoTracking();
        var normalizedName = name?.Trim();

        if (projectId.HasValue)
        {
            query = query.Where(feature => feature.ProjectId == projectId.Value);
        }

        if (!string.IsNullOrEmpty(normalizedName))
        {
            query = query.Where(feature => EF.Functions.ILike(feature.Name, $"%{normalizedName}%"));
        }

        if (status.HasValue)
        {
            query = query.Where(feature => feature.Status == status.Value);
        }

        return await query
            .OrderBy(feature => feature.Order)
            .ThenBy(feature => feature.Id)
            .ToListAsync(cancellationToken);
    }
}
