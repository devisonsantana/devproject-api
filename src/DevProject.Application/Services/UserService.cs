using DevProject.Application.DTOs.Auth;
using DevProject.Application.Interfaces;
using DevProject.Application.Interfaces.Mappers;
using DevProject.Domain.Interfaces;

namespace DevProject.Application.Services;

public sealed class UserService(
    IUserRepository repository,
    IUnitOfWork unitOfWork,
    IUserMapper mapper) : IUserService
{
    private readonly IUserRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUserMapper _mapper = mapper;

    public async Task<UserResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByIdAsync(id, cancellationToken);

        return user is null
            ? null
            : _mapper.ToResponse(user);
    }

    public async Task<IEnumerable<UserResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var users = await _repository.GetAllAsync(cancellationToken);

        return users.Select(_mapper.ToResponse).ToArray();
    }

    public async Task<UserResponse> CreateAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Password);

        var passwordHash = global::BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = _mapper.ToEntity(request, passwordHash);
        _repository.Add(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.ToResponse(user);
    }
}
