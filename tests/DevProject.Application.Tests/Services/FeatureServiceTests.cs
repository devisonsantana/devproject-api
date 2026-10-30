using DevProject.Application.DTOs.Features;
using DevProject.Application.Interfaces.Mappers;
using DevProject.Application.Queries.Features;
using DevProject.Application.Services;
using DevProject.Domain.Entities;
using DevProject.Domain.Enums;
using DevProject.Domain.Interfaces;
using Moq;

namespace DevProject.Application.Tests.Services;

public sealed class FeatureServiceTests
{
    private readonly Mock<IFeatureRepository> _repository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IFeatureMapper> _mapper = new();
    private readonly FeatureService _service;

    public FeatureServiceTests()
    {
        _service = new FeatureService(_repository.Object, _unitOfWork.Object, _mapper.Object);
    }

    [Fact]
    public async Task GetByIdAsync_returns_mapped_feature_when_found()
    {
        var id = Guid.NewGuid();
        var feature = new Feature(Guid.NewGuid(), "Feature");
        var response = CreateResponse(feature);
        _repository.Setup(repository => repository.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(feature);
        _mapper.Setup(mapper => mapper.ToResponse(feature)).Returns(response);

        var result = await _service.GetByIdAsync(id);

        Assert.Equal(response, result);
        _repository.Verify(repository =>
            repository.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _mapper.Verify(mapper => mapper.ToResponse(feature), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_returns_null_when_feature_is_missing()
    {
        var id = Guid.NewGuid();
        _repository.Setup(repository => repository.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Feature?)null);

        var result = await _service.GetByIdAsync(id);

        Assert.Null(result);
        _mapper.Verify(mapper => mapper.ToResponse(It.IsAny<Feature>()), Times.Never);
    }

    [Fact]
    public async Task GetAllAsync_maps_all_features()
    {
        var features = new[]
        {
            new Feature(Guid.NewGuid(), "First"),
            new Feature(Guid.NewGuid(), "Second")
        };
        _repository.Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(features);
        foreach (var feature in features)
        {
            _mapper.Setup(mapper => mapper.ToResponse(feature)).Returns(CreateResponse(feature));
        }

        var result = (await _service.GetAllAsync()).ToArray();

        Assert.Equal(2, result.Length);
        Assert.Equal(features.Select(feature => feature.Id), result.Select(response => response.Id));
        foreach (var feature in features)
        {
            _mapper.Verify(mapper => mapper.ToResponse(feature), Times.Once);
        }
    }

    [Fact]
    public async Task SearchAsync_forwards_filters_and_maps_features()
    {
        var projectId = Guid.NewGuid();
        var query = new FeatureSearchQuery(projectId, "auth", FeatureStatus.InProgress);
        var feature = new Feature(projectId, "Authentication");
        var response = CreateResponse(feature);
        _repository.Setup(repository => repository.GetFilteredAsync(
                projectId,
                "auth",
                FeatureStatus.InProgress,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([feature]);
        _mapper.Setup(mapper => mapper.ToResponse(feature)).Returns(response);

        var result = (await _service.SearchAsync(query)).ToArray();

        Assert.Equal([response], result);
        _repository.Verify(repository => repository.GetFilteredAsync(
            projectId,
            "auth",
            FeatureStatus.InProgress,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_maps_adds_saves_and_returns_feature()
    {
        var projectId = Guid.NewGuid();
        var request = new CreateFeatureRequest("Feature", "Description", 5);
        var feature = new Feature(projectId, request.Name, request.Description, request.Order);
        var response = CreateResponse(feature);
        using var cancellationSource = new CancellationTokenSource();
        var cancellationToken = cancellationSource.Token;
        _mapper.Setup(mapper => mapper.ToEntity(projectId, request.Order, request)).Returns(feature);
        _mapper.Setup(mapper => mapper.ToResponse(feature)).Returns(response);
        _unitOfWork.Setup(unitOfWork => unitOfWork.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(projectId, request, cancellationToken);

        Assert.Equal(response, result);
        _mapper.Verify(mapper => mapper.ToEntity(projectId, request.Order, request), Times.Once);
        _repository.Verify(repository => repository.Add(feature), Times.Once);
        _unitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(cancellationToken), Times.Once);
        _mapper.Verify(mapper => mapper.ToResponse(feature), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_rejects_null_request_without_persisting()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _service.CreateAsync(Guid.NewGuid(), null!));

        _repository.Verify(repository => repository.Add(It.IsAny<Feature>()), Times.Never);
        _unitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static FeatureResponse CreateResponse(Feature feature) =>
        new(feature.Id, feature.ProjectId, feature.Name, feature.Description, feature.Status,
            feature.Order, feature.CreatedAt, feature.UpdatedAt);
}
