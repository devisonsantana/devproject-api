using DevProject.Application.DTOs.Features;
using DevProject.Application.Interfaces;
using DevProject.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DevProject.Api.Controllers;

[ApiController]
[Route("api/features")]
public sealed class FeaturesController(IFeatureService featureService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<FeatureResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FeatureResponse>>> Get(
        [FromQuery] Guid? projectId,
        [FromQuery] string? name,
        [FromQuery] FeatureStatus? status,
        CancellationToken cancellationToken)
    {
        var features = projectId is null && string.IsNullOrWhiteSpace(name) && status is null
            ? await featureService.GetAllAsync(cancellationToken)
            : await featureService.SearchAsync(
                new(projectId, name, status),
                cancellationToken);

        return Ok(features);
    }

    [HttpGet("{id:guid}", Name = "GetFeatureById")]
    [ProducesResponseType(typeof(FeatureResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FeatureResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var feature = await featureService.GetByIdAsync(id, cancellationToken);
        return feature is null ? NotFound() : Ok(feature);
    }

    [HttpPost("~/api/projects/{projectId:guid}/features")]
    [ProducesResponseType(typeof(FeatureResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FeatureResponse>> Create(
        Guid projectId,
        CreateFeatureRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var feature = await featureService.CreateAsync(projectId, request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = feature.Id }, feature);
        }
        catch (ArgumentException exception)
        {
            return ValidationProblem("Invalid feature", exception.Message);
        }
    }

    private static ObjectResult ValidationProblem(string title, string detail) =>
        new(new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = title,
            Detail = detail
        })
        {
            StatusCode = StatusCodes.Status400BadRequest,
            ContentTypes = { "application/problem+json" }
        };
}
