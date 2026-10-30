using DevProject.Domain.Enums;

namespace DevProject.Application.Queries.Tasks;

public sealed record ProjectTaskSearchQuery(
    Guid? ProjectId = null,
    Guid? FeatureId = null,
    string? Search = null,
    ProjectTaskStatus? Status = null,
    TaskPriority? Priority = null);
