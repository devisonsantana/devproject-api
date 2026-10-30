using DevProject.Application.DTOs.Features;
using DevProject.Domain.Enums;

namespace DevProject.Application.Tests.DTOs.Features;

public sealed class FeatureDtoTests
{
    [Fact]
    public void CreateFeatureRequest_preserves_values_and_default_order()
    {
        var request = new CreateFeatureRequest("Feature", null);

        Assert.Equal("Feature", request.Name);
        Assert.Null(request.Description);
        Assert.Equal(0, request.Order);
    }

    [Fact]
    public void CreateFeatureRequest_preserves_explicit_order_and_description()
    {
        var request = new CreateFeatureRequest("Feature", "Description", 4);

        Assert.Equal("Description", request.Description);
        Assert.Equal(4, request.Order);
    }

    [Fact]
    public void UpdateFeatureRequest_preserves_all_values()
    {
        var request = new UpdateFeatureRequest("Feature", null, FeatureStatus.OnHold, 9);

        Assert.Equal("Feature", request.Name);
        Assert.Null(request.Description);
        Assert.Equal(FeatureStatus.OnHold, request.Status);
        Assert.Equal(9, request.Order);
    }

    [Fact]
    public void FeatureResponse_preserves_all_values()
    {
        var id = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow;
        var updatedAt = createdAt.AddMinutes(1);

        var response = new FeatureResponse(
            id,
            projectId,
            "Feature",
            null,
            FeatureStatus.InProgress,
            3,
            createdAt,
            updatedAt);

        Assert.Equal(id, response.Id);
        Assert.Equal(projectId, response.ProjectId);
        Assert.Equal("Feature", response.Name);
        Assert.Null(response.Description);
        Assert.Equal(FeatureStatus.InProgress, response.Status);
        Assert.Equal(3, response.Order);
        Assert.Equal(createdAt, response.CreatedAt);
        Assert.Equal(updatedAt, response.UpdatedAt);
    }
}
