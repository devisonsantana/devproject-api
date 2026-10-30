using DevProject.Application.DTOs.Projects;
using DevProject.Application.Interfaces.Mappers;
using DevProject.Domain.Entities;

namespace DevProject.Application.Mappers;

public class ProjectMapper : IProjectMapper
{
    public Project ToEntity(CreateProjectRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new Project(
            request.Name,
            request.Description
        );
    }

    public ProjectResponse ToResponse(Project project)
    {
        ArgumentNullException.ThrowIfNull(project);

        return new ProjectResponse(
           project.Id,
           project.Name,
           project.Description,
           project.Status,
           project.CreatedAt,
           project.UpdatedAt
       );
    }

    public void UpdateEntity(Project project, UpdateProjectRequest request)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(request);

        project.UpdateDetails(request.Name, request.Description);
    }
}
