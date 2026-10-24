using DevProject.Domain.Entities;
using DevProject.Domain.Interfaces;
using DevProject.Infrastructure.Context;

namespace DevProject.Infrastructure.Persistence.Repositories;

public sealed class ProjectTaskRepository : Repository<ProjectTask>, IProjectTaskRepository
{
    public ProjectTaskRepository(AppDbContext context) : base(context) { }
}
