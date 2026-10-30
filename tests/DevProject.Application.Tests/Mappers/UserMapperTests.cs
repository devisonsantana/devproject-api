using DevProject.Application.DTOs.Auth;
using DevProject.Application.Mappers;
using DevProject.Domain.Entities;

namespace DevProject.Application.Tests.Mappers;

public sealed class UserMapperTests
{
    private readonly UserMapper _mapper = new();

    [Fact]
    public void ToEntity_maps_registration_profile_and_precomputed_password_hash()
    {
        var request = new RegisterRequest("Dev User", "user@example.com", "plain-password");

        var user = _mapper.ToEntity(request, "precomputed-password-hash");

        Assert.Equal("Dev User", user.Name);
        Assert.Equal("user@example.com", user.Email);
        Assert.Equal("precomputed-password-hash", user.PasswordHash);
        Assert.NotEqual(request.Password, user.PasswordHash);
        Assert.True(user.IsActive);
    }

    [Fact]
    public void ToEntity_rejects_null_request_or_blank_hash()
    {
        var request = new RegisterRequest("Dev User", "user@example.com", "password");

        Assert.Throws<ArgumentNullException>(() => _mapper.ToEntity(null!, "hash"));
        Assert.Throws<ArgumentException>(() => _mapper.ToEntity(request, " "));
    }

    [Fact]
    public void ToResponse_maps_public_user_fields_without_password_hash()
    {
        var user = new User("Dev User", "user@example.com", "secret-hash");
        user.Deactivate();

        var response = _mapper.ToResponse(user);

        Assert.Equal(user.Id, response.Id);
        Assert.Equal(user.Name, response.Name);
        Assert.Equal(user.Email, response.Email);
        Assert.Equal(user.IsActive, response.IsActive);
        Assert.DoesNotContain(
            typeof(UserResponse).GetProperties(),
            property => property.Name.Contains("Password", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ToResponse_rejects_null_user()
    {
        Assert.Throws<ArgumentNullException>(() => _mapper.ToResponse(null!));
    }
}
