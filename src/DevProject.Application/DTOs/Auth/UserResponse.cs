namespace DevProject.Application.DTOs.Auth;

/// <summary>Public user profile data returned by the API.</summary>
public sealed record UserResponse(Guid Id, string Name, string Email, bool IsActive);
