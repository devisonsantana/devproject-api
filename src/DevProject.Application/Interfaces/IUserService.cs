using DevProject.Application.DTOs.Auth;

namespace DevProject.Application.Interfaces;

public interface IUserService
{
    Task<UserResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<UserResponse>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<UserResponse> CreateAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default);
}
