using DevProject.Application.DTOs.Auth;
using DevProject.Application.Interfaces.Mappers;
using DevProject.Application.Services;
using DevProject.Domain.Entities;
using DevProject.Domain.Interfaces;
using Moq;

namespace DevProject.Application.Tests.Services;

public sealed class UserServiceTests
{
    private readonly Mock<IUserRepository> _repository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IUserMapper> _mapper = new();
    private readonly UserService _service;

    public UserServiceTests()
    {
        _service = new UserService(_repository.Object, _unitOfWork.Object, _mapper.Object);
    }

    [Fact]
    public async Task GetByIdAsync_returns_mapped_user_when_found()
    {
        var id = Guid.NewGuid();
        var user = new User("User", "user@example.com", "hash");
        var response = CreateResponse(user);
        _repository.Setup(repository => repository.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mapper.Setup(mapper => mapper.ToResponse(user)).Returns(response);

        var result = await _service.GetByIdAsync(id);

        Assert.Equal(response, result);
        _mapper.Verify(mapper => mapper.ToResponse(user), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_returns_null_when_user_is_missing()
    {
        var id = Guid.NewGuid();
        _repository.Setup(repository => repository.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await _service.GetByIdAsync(id);

        Assert.Null(result);
        _mapper.Verify(mapper => mapper.ToResponse(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task GetAllAsync_maps_all_users()
    {
        var users = new[]
        {
            new User("First", "first@example.com", "hash1"),
            new User("Second", "second@example.com", "hash2")
        };
        _repository.Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);
        foreach (var user in users)
        {
            _mapper.Setup(mapper => mapper.ToResponse(user)).Returns(CreateResponse(user));
        }

        var result = (await _service.GetAllAsync()).ToArray();

        Assert.Equal(2, result.Length);
        Assert.Equal(users.Select(user => user.Id), result.Select(response => response.Id));
        foreach (var user in users)
        {
            _mapper.Verify(mapper => mapper.ToResponse(user), Times.Once);
        }
    }

    [Fact]
    public async Task CreateAsync_hashes_password_before_mapping_and_persisting()
    {
        const string password = "strong-password";
        var request = new RegisterRequest("New User", "user@example.com", password);
        User? mappedUser = null;
        _mapper.Setup(mapper => mapper.ToEntity(request, It.IsAny<string>()))
            .Callback<RegisterRequest, string>((_, hash) =>
            {
                Assert.NotEqual(password, hash);
                Assert.True(global::BCrypt.Net.BCrypt.Verify(password, hash));
                mappedUser = new User(request.Name, request.Email, hash);
            })
            .Returns(() => mappedUser!);
        _mapper.Setup(mapper => mapper.ToResponse(It.IsAny<User>()))
            .Returns((User user) => CreateResponse(user));
        using var cancellationSource = new CancellationTokenSource();
        var cancellationToken = cancellationSource.Token;
        _unitOfWork.Setup(unitOfWork => unitOfWork.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(request, cancellationToken);

        Assert.NotNull(mappedUser);
        Assert.Equal(mappedUser.Id, result.Id);
        Assert.NotEqual(password, mappedUser.PasswordHash);
        _mapper.Verify(mapper => mapper.ToEntity(request, It.Is<string>(hash =>
            !string.IsNullOrWhiteSpace(hash) && global::BCrypt.Net.BCrypt.Verify(password, hash))), Times.Once);
        _repository.Verify(repository => repository.Add(mappedUser), Times.Once);
        _unitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_rejects_null_request_or_blank_password_without_persisting()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateAsync(null!));
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CreateAsync(new RegisterRequest("User", "user@example.com", " ")));

        _mapper.Verify(mapper => mapper.ToEntity(
            It.IsAny<RegisterRequest>(), It.IsAny<string>()), Times.Never);
        _repository.Verify(repository => repository.Add(It.IsAny<User>()), Times.Never);
        _unitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static UserResponse CreateResponse(User user) =>
        new(user.Id, user.Name, user.Email, user.IsActive);
}
