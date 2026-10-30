using DevProject.Domain.Enums;

namespace DevProject.Application.DTOs.Tasks;

public sealed record UpdateTaskStatusRequest(ProjectTaskStatus Status);
