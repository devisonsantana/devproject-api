using DevProject.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace DevProject.Api.Tests;

public sealed class ApiFactory : WebApplicationFactory<Program>
{
    public Mock<IProjectService> Projects { get; } = new();

    public Mock<IFeatureService> Features { get; } = new();

    public Mock<IProjectTaskService> Tasks { get; } = new();

    public Mock<IUserService> Users { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IProjectService>();
            services.RemoveAll<IFeatureService>();
            services.RemoveAll<IProjectTaskService>();
            services.RemoveAll<IUserService>();

            services.AddSingleton(Projects.Object);
            services.AddSingleton(Features.Object);
            services.AddSingleton(Tasks.Object);
            services.AddSingleton(Users.Object);
        });
    }
}
