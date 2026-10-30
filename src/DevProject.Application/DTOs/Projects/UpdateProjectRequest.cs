namespace DevProject.Application.DTOs.Projects;

/// <summary>Descriptive values used to update a project.</summary>
public sealed record UpdateProjectRequest(string Name, string? Description);
