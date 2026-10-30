using DevProject.Application.DTOs.Features;
using DevProject.Application.Mappers;
using DevProject.Domain.Entities;
using DevProject.Domain.Enums;

namespace DevProject.Application.Tests.Mappers;

public sealed class FeatureMapperTests
{
    private readonly FeatureMapper _mapper = new();

    [Fact]
    public void ToEntity_maps_request_and_route_values()
    {
        var projectId = Guid.NewGuid();
        var request = new CreateFeatureRequest("Feature", "Description", Order: 88);

        var feature = _mapper.ToEntity(projectId, 4, request);

        Assert.Equal(projectId, feature.ProjectId);
        Assert.Equal("Feature", feature.Name);
        Assert.Equal("Description", feature.Description);
        Assert.Equal(FeatureStatus.Planned, feature.Status);
        Assert.Equal(4, feature.Order);
    }

    [Fact]
    public void ToEntity_rejects_null_request()
    {
        Assert.Throws<ArgumentNullException>(() => _mapper.ToEntity(Guid.NewGuid(), 0, null!));
    }

    [Fact]
    public void ToResponse_maps_all_feature_fields()
    {
        var feature = new Feature(Guid.NewGuid(), "Feature", "Description", 5);
        feature.ChangeStatus(FeatureStatus.InProgress);

        var response = _mapper.ToResponse(feature);

        Assert.Equal(feature.Id, response.Id);
        Assert.Equal(feature.ProjectId, response.ProjectId);
        Assert.Equal(feature.Name, response.Name);
        Assert.Equal(feature.Description, response.Description);
        Assert.Equal(feature.Status, response.Status);
        Assert.Equal(feature.Order, response.Order);
        Assert.Equal(feature.CreatedAt, response.CreatedAt);
        Assert.Equal(feature.UpdatedAt, response.UpdatedAt);
    }

    [Fact]
    public void ToResponse_rejects_null_feature()
    {
        Assert.Throws<ArgumentNullException>(() => _mapper.ToResponse(null!));
    }

    [Fact]
    public void UpdateEntity_applies_details_status_and_order()
    {
        var feature = new Feature(Guid.NewGuid(), "Old", "Old description", 1);
        var request = new UpdateFeatureRequest(
            "Updated",
            "New description",
            FeatureStatus.Completed,
            7);

        _mapper.UpdateEntity(feature, request);

        Assert.Equal("Updated", feature.Name);
        Assert.Equal("New description", feature.Description);
        Assert.Equal(FeatureStatus.Completed, feature.Status);
        Assert.Equal(7, feature.Order);
    }

    [Fact]
    public void UpdateEntity_does_not_partially_update_when_status_is_invalid()
    {
        var feature = new Feature(Guid.NewGuid(), "Original", "Original description", 2);
        var request = new UpdateFeatureRequest(
            "Changed",
            "Changed description",
            (FeatureStatus)999,
            3);

        Assert.Throws<ArgumentException>(() => _mapper.UpdateEntity(feature, request));
        Assert.Equal("Original", feature.Name);
        Assert.Equal("Original description", feature.Description);
        Assert.Equal(FeatureStatus.Planned, feature.Status);
        Assert.Equal(2, feature.Order);
        Assert.Null(feature.UpdatedAt);
    }

    [Fact]
    public void UpdateEntity_rejects_null_arguments()
    {
        var feature = new Feature(Guid.NewGuid(), "Feature");
        var request = new UpdateFeatureRequest("Updated", null, FeatureStatus.Planned, 1);

        Assert.Throws<ArgumentNullException>(() => _mapper.UpdateEntity(null!, request));
        Assert.Throws<ArgumentNullException>(() => _mapper.UpdateEntity(feature, null!));
    }
}
