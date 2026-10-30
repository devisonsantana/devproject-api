using DevProject.Application.DTOs.Projects;
using DevProject.Domain.Entities;

namespace DevProject.Application.Interfaces.Mappers;

public interface IProjectMapper
{
    Project ToEntity(CreateProjectRequest request);
    ProjectResponse ToResponse(Project project);
    void UpdateEntity(Project project, UpdateProjectRequest request);
}
