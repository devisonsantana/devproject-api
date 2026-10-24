using DevProject.Domain.Entities;
using DevProject.Domain.Interfaces;
using DevProject.Infrastructure.Context;

namespace DevProject.Infrastructure.Persistence.Repositories;

public sealed class FeatureRepository : Repository<Feature>, IFeatureRepository
{
    public FeatureRepository(AppDbContext context) : base(context) { }
}
