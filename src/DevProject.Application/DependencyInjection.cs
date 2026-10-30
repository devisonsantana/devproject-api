using DevProject.Application.Interfaces;
using DevProject.Application.Interfaces.Mappers;
using DevProject.Application.Mappers;
using DevProject.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DevProject.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IFeatureService, FeatureService>();
        services.AddScoped<IProjectTaskService, ProjectTaskService>();

        services.AddScoped<IUserMapper, UserMapper>();
        services.AddScoped<IProjectMapper, ProjectMapper>();
        services.AddScoped<IFeatureMapper, FeatureMapper>();
        services.AddScoped<IProjectTaskMapper, ProjectTaskMapper>();

        return services;
    }
}