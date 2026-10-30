using DevProject.Application.DTOs.Tasks;
using DevProject.Application.Interfaces.Mappers;
using DevProject.Application.Queries.Tasks;
using DevProject.Application.Services;
using DevProject.Domain.Entities;
using DevProject.Domain.Enums;
using DevProject.Domain.Interfaces;
using Moq;

namespace DevProject.Application.Tests.Services;

public sealed class ProjectTaskServiceTests
{
    private readonly Mock<IProjectTaskRepository> _repository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IProjectTaskMapper> _mapper = new();
    private readonly ProjectTaskService _service;

    public ProjectTaskServiceTests()
    {
        _service = new ProjectTaskService(_repository.Object, _unitOfWork.Object, _mapper.Object);
    }

    [Fact]
    public async Task GetByIdAsync_returns_mapped_task_when_found()
    {
        var id = Guid.NewGuid();
        var task = new ProjectTask(Guid.NewGuid(), "Task");
        var response = CreateResponse(task);
        _repository.Setup(repository => repository.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);
        _mapper.Setup(mapper => mapper.ToResponse(task)).Returns(response);

        var result = await _service.GetByIdAsync(id);

        Assert.Equal(response, result);
        _repository.Verify(repository =>
            repository.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _mapper.Verify(mapper => mapper.ToResponse(task), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_returns_null_when_task_is_missing()
    {
        var id = Guid.NewGuid();
        _repository.Setup(repository => repository.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectTask?)null);

        var result = await _service.GetByIdAsync(id);

        Assert.Null(result);
        _mapper.Verify(mapper => mapper.ToResponse(It.IsAny<ProjectTask>()), Times.Never);
    }

    [Fact]
    public async Task GetAllAsync_maps_all_tasks()
    {
        var tasks = new[]
        {
            new ProjectTask(Guid.NewGuid(), "First"),
            new ProjectTask(Guid.NewGuid(), "Second")
        };
        _repository.Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks);
        foreach (var task in tasks)
        {
            _mapper.Setup(mapper => mapper.ToResponse(task)).Returns(CreateResponse(task));
        }

        var result = (await _service.GetAllAsync()).ToArray();

        Assert.Equal(2, result.Length);
        Assert.Equal(tasks.Select(task => task.Id), result.Select(response => response.Id));
        foreach (var task in tasks)
        {
            _mapper.Verify(mapper => mapper.ToResponse(task), Times.Once);
        }
    }

    [Fact]
    public async Task SearchAsync_forwards_filters_and_maps_tasks()
    {
        var projectId = Guid.NewGuid();
        var featureId = Guid.NewGuid();
        var query = new ProjectTaskSearchQuery(
            projectId,
            featureId,
            "login",
            ProjectTaskStatus.Blocked,
            TaskPriority.High);
        var task = new ProjectTask(featureId, "Implement login");
        var response = CreateResponse(task);
        _repository.Setup(repository => repository.GetFilteredAsync(
                projectId,
                featureId,
                "login",
                ProjectTaskStatus.Blocked,
                TaskPriority.High,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([task]);
        _mapper.Setup(mapper => mapper.ToResponse(task)).Returns(response);

        var result = (await _service.SearchAsync(query)).ToArray();

        Assert.Equal([response], result);
        _repository.Verify(repository => repository.GetFilteredAsync(
            projectId,
            featureId,
            "login",
            ProjectTaskStatus.Blocked,
            TaskPriority.High,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_maps_adds_saves_and_returns_task()
    {
        var featureId = Guid.NewGuid();
        var request = new CreateProjectTaskRequest("Task", "Description", Order: 4);
        var task = new ProjectTask(featureId, request.Title, request.Description, request.Priority,
            request.DueDate, request.Order);
        var response = CreateResponse(task);
        using var cancellationSource = new CancellationTokenSource();
        var cancellationToken = cancellationSource.Token;
        _mapper.Setup(mapper => mapper.ToEntity(featureId, request)).Returns(task);
        _mapper.Setup(mapper => mapper.ToResponse(task)).Returns(response);
        _unitOfWork.Setup(unitOfWork => unitOfWork.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(featureId, request, cancellationToken);

        Assert.Equal(response, result);
        _mapper.Verify(mapper => mapper.ToEntity(featureId, request), Times.Once);
        _repository.Verify(repository => repository.Add(task), Times.Once);
        _unitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(cancellationToken), Times.Once);
        _mapper.Verify(mapper => mapper.ToResponse(task), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_rejects_null_request_without_persisting()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _service.CreateAsync(Guid.NewGuid(), null!));

        _repository.Verify(repository => repository.Add(It.IsAny<ProjectTask>()), Times.Never);
        _unitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static ProjectTaskResponse CreateResponse(ProjectTask task) =>
        new(task.Id, task.FeatureId, task.Title, task.Description, task.Status, task.Priority,
            task.DueDate, task.CompletedAt, task.Order, task.CreatedAt, task.UpdatedAt);
}
