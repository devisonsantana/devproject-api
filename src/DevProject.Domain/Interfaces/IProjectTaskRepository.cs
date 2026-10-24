using DevProject.Domain.Entities;

namespace DevProject.Domain.Interfaces;

public interface IProjectTaskRepository : IRepository<ProjectTask>
{
    // get by feature id, project id, status
}
