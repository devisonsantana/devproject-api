using DevProject.Application.DTOs.Auth;
using DevProject.Application.Interfaces.Mappers;
using DevProject.Domain.Entities;

namespace DevProject.Application.Mappers;

public class UserMapper : IUserMapper
{
    public User ToEntity(RegisterRequest request, string passwordHash)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

        return new(
            request.Name,
            request.Email,
            passwordHash
        );
    }

    public UserResponse ToResponse(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        return new(
            user.Id,
            user.Name,
            user.Email,
            user.IsActive
        );
    }
}
