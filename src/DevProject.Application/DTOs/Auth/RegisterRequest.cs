namespace DevProject.Application.DTOs.Auth;

/// <summary>Credentials and profile details used to register a user.</summary>
public sealed record RegisterRequest(string Name, string Email, string Password);
