using DevProject.Application.DTOs.Tasks;
using DevProject.Domain.Entities;
using DevProject.Domain.Enums;

namespace DevProject.Application.Interfaces.Mappers;

public interface IProjectTaskMapper
{

    ProjectTask ToEntity(Guid featureId, CreateProjectTaskRequest request);
    ProjectTaskResponse ToResponse(ProjectTask task);
    void UpdateEntity(ProjectTask task, UpdateProjectTaskRequest request);
    void UpdateStatus(ProjectTask task, ProjectTaskStatus status);
    void UpdateOrder(ProjectTask task, int order);
}
