using DevProject.Application.DTOs.Features;
using DevProject.Application.Interfaces;
using DevProject.Application.Interfaces.Mappers;
using DevProject.Domain.Interfaces;

namespace DevProject.Application.Services;

public sealed class FeatureService(
    IFeatureRepository repository,
    IUnitOfWork unitOfWork,
    IFeatureMapper mapper) : IFeatureService
{
    private readonly IFeatureRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IFeatureMapper _mapper = mapper;

    public async Task<FeatureResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var feature = await _repository.GetByIdAsync(id, cancellationToken);

        return feature is null
            ? null
            : _mapper.ToResponse(feature);
    }

    public async Task<IEnumerable<FeatureResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var features = await _repository.GetAllAsync(cancellationToken);

        return features.Select(_mapper.ToResponse).ToArray();
    }

    public async Task<FeatureResponse> CreateAsync(
        Guid projectId,
        CreateFeatureRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var feature = _mapper.ToEntity(projectId, request.Order, request);
        _repository.Add(feature);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.ToResponse(feature);
    }
}
