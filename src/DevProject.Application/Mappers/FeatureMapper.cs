using DevProject.Application.DTOs.Features;
using DevProject.Application.Interfaces.Mappers;
using DevProject.Domain.Entities;
using DevProject.Domain.Enums;

namespace DevProject.Application.Mappers;

public class FeatureMapper : IFeatureMapper
{
    public Feature ToEntity(Guid projectId, int order, CreateFeatureRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new(
            projectId,
            request.Name,
            request.Description,
            order
        );
    }

    public FeatureResponse ToResponse(Feature feature)
    {
        ArgumentNullException.ThrowIfNull(feature);

        return new(
            feature.Id,
            feature.ProjectId,
            feature.Name,
            feature.Description,
            feature.Status,
            feature.Order,
            feature.CreatedAt,
            feature.UpdatedAt
        );

    }

    public void UpdateEntity(Feature feature, UpdateFeatureRequest request)
    {
        ArgumentNullException.ThrowIfNull(feature);
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Feature name is required.", nameof(request));
        }

        if (!Enum.IsDefined(request.Status))
        {
            throw new ArgumentException("Feature status is invalid.", nameof(request));
        }

        feature.UpdateDetails(request.Name, request.Description);
        feature.ChangeStatus(request.Status);
        feature.ChangeOrder(request.Order);
    }
}
