using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using DevProject.Application.DTOs.Auth;
using DevProject.Application.DTOs.Features;
using DevProject.Application.DTOs.Projects;
using DevProject.Application.DTOs.Tasks;
using DevProject.Application.Queries.Features;
using DevProject.Application.Queries.Projects;
using DevProject.Application.Queries.Tasks;
using DevProject.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Moq;

namespace DevProject.Api.Tests.Controllers;

public sealed class ApiControllerTests
{
    [Fact]
    public async Task Projects_get_returns_project_responses_and_filters_by_query()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        var response = new ProjectResponse(
            Guid.NewGuid(), "Active project", null, ProjectStatus.Active, DateTimeOffset.UtcNow, null);
        factory.Projects.Setup(service => service.SearchAsync(
                It.IsAny<ProjectSearchQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([response]);

        var result = await client.GetAsync("/api/projects?name=active&status=Active");

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        var projects = await result.Content.ReadFromJsonAsync<ProjectResponse[]>(JsonOptions);
        Assert.NotNull(projects);
        Assert.Single(projects);
        Assert.Equal("Active project", projects[0].Name);
        Assert.Contains("\"status\":\"Active\"", await result.Content.ReadAsStringAsync(),
            StringComparison.Ordinal);
        factory.Projects.Verify(service => service.SearchAsync(
            It.Is<ProjectSearchQuery>(query =>
                query.Name == "active" && query.Status == ProjectStatus.Active),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Projects_get_by_id_returns_not_found_when_missing()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        var id = Guid.NewGuid();
        factory.Projects.Setup(service => service.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectResponse?)null);

        var response = await client.GetAsync($"/api/projects/{id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Project_create_returns_created_status_and_location()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        var created = new ProjectResponse(
            Guid.NewGuid(), "New project", null, ProjectStatus.Planned, DateTimeOffset.UtcNow, null);
        factory.Projects.Setup(service => service.CreateAsync(
                It.IsAny<CreateProjectRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(created);

        var response = await client.PostAsJsonAsync(
            "/api/projects",
            new CreateProjectRequest("New project", null));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal($"/api/projects/{created.Id}", response.Headers.Location?.AbsolutePath);
        var body = await response.Content.ReadFromJsonAsync<ProjectResponse>(JsonOptions);
        Assert.Equal(created.Id, body?.Id);
        factory.Projects.Verify(service => service.CreateAsync(
            It.Is<CreateProjectRequest>(request => request.Name == "New project"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Invalid_enum_query_returns_bad_request()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/projects?status=not-a-status");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        factory.Projects.Verify(service => service.GetAllAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Invalid_project_creation_returns_problem_details()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        factory.Projects.Setup(service => service.CreateAsync(
                It.IsAny<CreateProjectRequest>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Project name is required."));

        var response = await client.PostAsJsonAsync(
            "/api/projects",
            new CreateProjectRequest(" ", null));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var problem = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(StatusCodes.Status400BadRequest, problem.GetProperty("status").GetInt32());
        Assert.Equal("Invalid project", problem.GetProperty("title").GetString());
    }

    [Fact]
    public async Task Features_get_passes_filter_query_to_application()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        var projectId = Guid.NewGuid();
        factory.Features.Setup(service => service.SearchAsync(
                It.IsAny<FeatureSearchQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var response = await client.GetAsync(
            $"/api/features?projectId={projectId}&name=auth&status=InProgress");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        factory.Features.Verify(service => service.SearchAsync(
            It.Is<FeatureSearchQuery>(query =>
                query.ProjectId == projectId
                && query.Name == "auth"
                && query.Status == FeatureStatus.InProgress),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Feature_create_uses_project_id_from_route()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        var projectId = Guid.NewGuid();
        var feature = new FeatureResponse(
            Guid.NewGuid(), projectId, "Feature", null, FeatureStatus.Planned, 2,
            DateTimeOffset.UtcNow, null);
        factory.Features.Setup(service => service.CreateAsync(
                projectId,
                It.IsAny<CreateFeatureRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(feature);

        var response = await client.PostAsJsonAsync(
            $"/api/projects/{projectId}/features",
            new CreateFeatureRequest("Feature", null, 2));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal($"/api/features/{feature.Id}", response.Headers.Location?.AbsolutePath);
        factory.Features.Verify(service => service.CreateAsync(
            projectId,
            It.Is<CreateFeatureRequest>(request => request.Name == "Feature"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Tasks_get_passes_search_and_status_filters_to_application()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        var featureId = Guid.NewGuid();
        factory.Tasks.Setup(service => service.SearchAsync(
                It.IsAny<ProjectTaskSearchQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var response = await client.GetAsync(
            $"/api/tasks?featureId={featureId}&search=login&status=Blocked&priority=High");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        factory.Tasks.Verify(service => service.SearchAsync(
            It.Is<ProjectTaskSearchQuery>(query =>
                query.FeatureId == featureId
                && query.Search == "login"
                && query.Status == ProjectTaskStatus.Blocked
                && query.Priority == TaskPriority.High),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Task_create_uses_feature_id_from_route()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        var featureId = Guid.NewGuid();
        var task = new ProjectTaskResponse(
            Guid.NewGuid(), featureId, "Task", null, ProjectTaskStatus.Todo, TaskPriority.Medium,
            null, null, 0, DateTimeOffset.UtcNow, null);
        factory.Tasks.Setup(service => service.CreateAsync(
                featureId,
                It.IsAny<CreateProjectTaskRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        var response = await client.PostAsJsonAsync(
            $"/api/features/{featureId}/tasks",
            new CreateProjectTaskRequest("Task", null));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal($"/api/tasks/{task.Id}", response.Headers.Location?.AbsolutePath);
    }

    [Fact]
    public async Task User_registration_returns_created_user_without_password_fields()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        var user = new UserResponse(Guid.NewGuid(), "New user", "user@example.com", true);
        factory.Users.Setup(service => service.CreateAsync(
                It.IsAny<RegisterRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var response = await client.PostAsJsonAsync(
            "/api/users",
            new RegisterRequest("New user", "user@example.com", "password"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal($"/api/users/{user.Id}", response.Headers.Location?.OriginalString);
        var body = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(body);
        Assert.False(json.RootElement.TryGetProperty("password", out _));
        Assert.False(json.RootElement.TryGetProperty("passwordHash", out _));
    }

    [Fact]
    public async Task Swagger_ui_and_openapi_document_are_available()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var uiResponse = await client.GetAsync("/swagger");
        var documentResponse = await client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, uiResponse.StatusCode);
        Assert.Contains("Swagger UI", await uiResponse.Content.ReadAsStringAsync(),
            StringComparison.OrdinalIgnoreCase);
        Assert.Equal(HttpStatusCode.OK, documentResponse.StatusCode);
        var document = await documentResponse.Content.ReadAsStringAsync();
        Assert.Contains("/api/projects", document, StringComparison.Ordinal);
        Assert.Contains("/api/features", document, StringComparison.Ordinal);
        Assert.Contains("/api/tasks", document, StringComparison.Ordinal);
    }

    private static JsonSerializerOptions JsonOptions
    {
        get
        {
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        }
    }
}
