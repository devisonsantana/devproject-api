using DevProject.Application.DTOs.Features;
using DevProject.Domain.Entities;

namespace DevProject.Application.Interfaces.Mappers;

public interface IFeatureMapper
{
    Feature ToEntity(Guid projectId, int order, CreateFeatureRequest request);
    FeatureResponse ToResponse(Feature feature);
    void UpdateEntity(Feature feature, UpdateFeatureRequest request);
}
