using DevProject.Domain.Enums;

namespace DevProject.Application.Queries.Features;

public sealed record FeatureSearchQuery(
    Guid? ProjectId = null,
    string? Name = null,
    FeatureStatus? Status = null);
