using DevProject.Application.DTOs.Projects;
using DevProject.Application.Interfaces.Mappers;
using DevProject.Application.Services;
using DevProject.Domain.Entities;
using DevProject.Domain.Interfaces;
using Moq;

namespace DevProject.Application.Tests.Services;

public sealed class ProjectServiceTests
{
    private readonly Mock<IProjectRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IProjectMapper> _mapperMock;

    private readonly ProjectService _service;

    public ProjectServiceTests()
    {
        _repositoryMock = new Mock<IProjectRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IProjectMapper>();

        _service = new ProjectService(
            _repositoryMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProject_WhenProjectExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var project = new Project("DevProject");
        var response = CreateResponse(project);

        _repositoryMock
            .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        _mapperMock
            .Setup(x => x.ToResponse(project))
            .Returns(response);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(response.Id, result.Id);
        Assert.Equal(response.Name, result.Name);

        _mapperMock.Verify(
            x => x.ToResponse(project),
            Times.Once
        );
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenProjectDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        Assert.Null(result);

        _mapperMock.Verify(
            x => x.ToResponse(It.IsAny<Project>()),
            Times.Never
        );
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllProjects()
    {
        // Arrange
        var projects = new List<Project>
        {
            new("Project 1"),
            new("Project 2")
        };

        _repositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(projects);

        foreach (var project in projects)
        {
            _mapperMock
                .Setup(x => x.ToResponse(project))
                .Returns(CreateResponse(project));
        }

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        var responses = result.ToList();

        Assert.Equal(2, responses.Count);

        Assert.Equal(projects[0].Id, responses[0].Id);
        Assert.Equal(projects[1].Id, responses[1].Id);

        _repositoryMock.Verify(
            x => x.GetAllAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateAndReturnProject()
    {
        // Arrange
        var request = new CreateProjectRequest(
            "DevProject",
            "Project management API"
        );

        var project = new Project(
            request.Name,
            request.Description
        );

        var response = CreateResponse(project);

        _mapperMock
            .Setup(x => x.ToEntity(request))
            .Returns(project);

        _mapperMock
            .Setup(x => x.ToResponse(project))
            .Returns(response);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(response.Id, result.Id);

        _repositoryMock.Verify(
            x => x.Add(project),
            Times.Once
        );

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _service.CreateAsync(null!)
        );

        _repositoryMock.Verify(
            x => x.Add(It.IsAny<Project>()),
            Times.Never
        );

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateProject_WhenProjectExists()
    {
        // Arrange
        var id = Guid.NewGuid();

        var project = new Project("Old Project");

        var request = new UpdateProjectRequest(
            "Updated Project",
            "Updated description"
        );

        var response = CreateResponse(project);

        _repositoryMock
            .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        _mapperMock
            .Setup(x => x.ToResponse(project))
            .Returns(response);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdateAsync(id, request);

        // Assert
        Assert.NotNull(result);

        _mapperMock.Verify(
            x => x.UpdateEntity(project, request),
            Times.Once
        );

        _repositoryMock.Verify(
            x => x.Update(project),
            Times.Once
        );

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenProjectDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();

        var request = new UpdateProjectRequest(
            "Updated Project",
            "Updated description"
        );

        _repositoryMock
            .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        // Act
        var result = await _service.UpdateAsync(id, request);

        // Assert
        Assert.Null(result);

        _mapperMock.Verify(
            x => x.UpdateEntity(It.IsAny<Project>(), It.IsAny<UpdateProjectRequest>()),
            Times.Never
        );

        _repositoryMock.Verify(
            x => x.Update(It.IsAny<Project>()),
            Times.Never
        );

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _service.UpdateAsync(Guid.NewGuid(), null!)
        );

        _repositoryMock.Verify(
            x => x.Update(It.IsAny<Project>()),
            Times.Never
        );

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveProject_WhenProjectExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var project = new Project("DevProject");

        _repositoryMock
            .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        Assert.True(result);

        _repositoryMock.Verify(
            x => x.Remove(project),
            Times.Once
        );

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenProjectDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        Assert.False(result);

        _repositoryMock.Verify(
            x => x.Remove(It.IsAny<Project>()),
            Times.Never
        );

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    private static ProjectResponse CreateResponse(Project project)
    {
        return new ProjectResponse(
            project.Id,
            project.Name,
            project.Description,
            project.Status,
            project.CreatedAt,
            project.UpdatedAt
        );
    }
}
