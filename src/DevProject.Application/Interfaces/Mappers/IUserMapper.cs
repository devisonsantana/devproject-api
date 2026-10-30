using DevProject.Application.DTOs.Auth;
using DevProject.Domain.Entities;

namespace DevProject.Application.Interfaces.Mappers;

public interface IUserMapper
{
    User ToEntity(RegisterRequest request, string passwordHash);
    UserResponse ToResponse(User user);
}
