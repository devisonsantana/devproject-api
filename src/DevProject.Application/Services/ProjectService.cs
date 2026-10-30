using DevProject.Application.DTOs.Projects;
using DevProject.Application.Interfaces;
using DevProject.Application.Interfaces.Mappers;
using DevProject.Domain.Interfaces;

namespace DevProject.Application.Services;

public sealed class ProjectService(
    IProjectRepository repository,
    IUnitOfWork unitOfWork,
    IProjectMapper mapper) : IProjectService
{
    private readonly IProjectRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IProjectMapper _mapper = mapper;

    public async Task<ProjectResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var project = await _repository.GetByIdAsync(id, cancellationToken);

        return project is null
            ? null
            : _mapper.ToResponse(project);
    }

    public async Task<IEnumerable<ProjectResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var projects = await _repository.GetAllAsync(cancellationToken);

        return projects.Select(_mapper.ToResponse);
    }

    public async Task<ProjectResponse> CreateAsync(
        CreateProjectRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var project = _mapper.ToEntity(request);

        _repository.Add(project);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.ToResponse(project);
    }

    public async Task<ProjectResponse?> UpdateAsync(
        Guid id,
        UpdateProjectRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var project = await _repository.GetByIdAsync(id, cancellationToken);

        if (project is null)
        {
            return null;
        }

        _mapper.UpdateEntity(project, request);

        _repository.Update(project);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.ToResponse(project);
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var project = await _repository.GetByIdAsync(id, cancellationToken);

        if (project is null)
        {
            return false;
        }

        _repository.Remove(project);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}