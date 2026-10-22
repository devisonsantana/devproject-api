using DevProject.Domain.Entities;
using DevProject.Domain.Enums;

namespace DevProject.Domain.Tests;

public sealed class FeatureTests
{
    [Fact]
    public void Feature_is_created_with_project_id_status_and_order()
    {
        var projectId = Guid.NewGuid();
        var feature = new Feature(projectId, "  Feature  ", " description ", order: 3);

        Assert.Equal(projectId, feature.ProjectId);
        Assert.Equal("Feature", feature.Name);
        Assert.Equal("description", feature.Description);
        Assert.Equal(FeatureStatus.Planned, feature.Status);
        Assert.Equal(3, feature.Order);
        Assert.Empty(feature.Tasks);
    }

    [Fact]
    public void Invalid_details_do_not_change_feature()
    {
        var feature = new Feature(Guid.NewGuid(), "Before", "Description");

        var action = () => feature.UpdateDetails(" ", null);

        Assert.Throws<ArgumentException>(action);
        Assert.Equal("Before", feature.Name);
        Assert.Equal("Description", feature.Description);
        Assert.Null(feature.UpdatedAt);
    }

    [Fact]
    public void Changing_status_and_order_updates_observable_values()
    {
        var feature = new Feature(Guid.NewGuid(), "Feature");

        feature.ChangeStatus(FeatureStatus.InProgress);
        feature.ChangeOrder(2);

        Assert.Equal(FeatureStatus.InProgress, feature.Status);
        Assert.Equal(2, feature.Order);
        Assert.NotNull(feature.UpdatedAt);
    }
}
