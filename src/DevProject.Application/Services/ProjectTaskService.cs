using DevProject.Application.DTOs.Tasks;
using DevProject.Application.Interfaces;
using DevProject.Application.Interfaces.Mappers;
using DevProject.Application.Queries.Tasks;
using DevProject.Domain.Interfaces;

namespace DevProject.Application.Services;

public sealed class ProjectTaskService(
    IProjectTaskRepository repository,
    IUnitOfWork unitOfWork,
    IProjectTaskMapper mapper) : IProjectTaskService
{
    private readonly IProjectTaskRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IProjectTaskMapper _mapper = mapper;

    public async Task<ProjectTaskResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var task = await _repository.GetByIdAsync(id, cancellationToken);

        return task is null
            ? null
            : _mapper.ToResponse(task);
    }

    public async Task<IEnumerable<ProjectTaskResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var tasks = await _repository.GetAllAsync(cancellationToken);

        return tasks.Select(_mapper.ToResponse).ToArray();
    }

    public async Task<IEnumerable<ProjectTaskResponse>> SearchAsync(
        ProjectTaskSearchQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var tasks = await _repository.GetFilteredAsync(
            query.ProjectId,
            query.FeatureId,
            query.Search,
            query.Status,
            query.Priority,
            cancellationToken);

        return tasks.Select(_mapper.ToResponse).ToArray();
    }

    public async Task<ProjectTaskResponse> CreateAsync(
        Guid featureId,
        CreateProjectTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var task = _mapper.ToEntity(featureId, request);
        _repository.Add(task);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.ToResponse(task);
    }
}
