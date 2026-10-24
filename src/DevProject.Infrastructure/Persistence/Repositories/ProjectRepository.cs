using DevProject.Domain.Entities;
using DevProject.Domain.Interfaces;
using DevProject.Infrastructure.Context;

namespace DevProject.Infrastructure.Persistence.Repositories;

public sealed class ProjectRepository : Repository<Project>, IProjectRepository
{
    public ProjectRepository(AppDbContext context) : base(context) { }
}
