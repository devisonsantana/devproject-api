namespace DevProject.Application.DTOs.Auth;

public sealed record UserResponse(Guid Id, string Name, string Email, bool IsActive);
