using DevProject.Domain.Enums;

namespace DevProject.Application.Queries.Projects;

public sealed record ProjectSearchQuery(string? Name = null, ProjectStatus? Status = null);
