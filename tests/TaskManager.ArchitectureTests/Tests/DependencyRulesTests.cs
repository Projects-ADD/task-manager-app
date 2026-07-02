using FluentAssertions;
using NetArchTest.Rules;

namespace TaskManager.ArchitectureTests.Tests;

public class DependencyRulesTests
{
    private const string DomainNamespace = "TaskManager.Domain";
    private const string ApplicationNamespace = "TaskManager.Application";
    private const string InfrastructureNamespace = "TaskManager.Infrastructure";
    private const string ApiNamespace = "TaskManager.Api";
    private const string ContractsNamespace = "TaskManager.Contracts";

    [Fact]
    public void Domain_ShouldNotDependOnApplication()
    {
        var result = Types
            .InAssembly(typeof(TaskManager.Domain.Common.BaseEntity).Assembly)
            .ShouldNot()
            .HaveDependencyOn(ApplicationNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Domain_ShouldNotDependOnInfrastructure()
    {
        var result = Types
            .InAssembly(typeof(TaskManager.Domain.Common.BaseEntity).Assembly)
            .ShouldNot()
            .HaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Domain_ShouldNotDependOnApi()
    {
        var result = Types
            .InAssembly(typeof(TaskManager.Domain.Common.BaseEntity).Assembly)
            .ShouldNot()
            .HaveDependencyOn(ApiNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Domain_ShouldNotDependOnContracts()
    {
        var result = Types
            .InAssembly(typeof(TaskManager.Domain.Common.BaseEntity).Assembly)
            .ShouldNot()
            .HaveDependencyOn(ContractsNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_ShouldNotDependOnInfrastructure()
    {
        var result = Types
            .InAssembly(typeof(TaskManager.Application.Features.Permissions.IPermissionService).Assembly)
            .That()
            .ResideInNamespace(ApplicationNamespace)
            .ShouldNot()
            .HaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_ShouldNotDependOnApi()
    {
        var result = Types
            .InAssembly(typeof(TaskManager.Application.Features.Permissions.IPermissionService).Assembly)
            .That()
            .ResideInNamespace(ApplicationNamespace)
            .ShouldNot()
            .HaveDependencyOn(ApiNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_ShouldNotDependOnContracts()
    {
        var result = Types
            .InAssembly(typeof(TaskManager.Application.Features.Permissions.IPermissionService).Assembly)
            .That()
            .ResideInNamespace(ApplicationNamespace)
            .ShouldNot()
            .HaveDependencyOn(ContractsNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOnApi()
    {
        var result = Types
            .InAssembly(typeof(TaskManager.Infrastructure.Persistence.TaskManagerDbContext).Assembly)
            .That()
            .ResideInNamespace(InfrastructureNamespace)
            .ShouldNot()
            .HaveDependencyOn(ApiNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOnContracts()
    {
        var result = Types
            .InAssembly(typeof(TaskManager.Infrastructure.Persistence.TaskManagerDbContext).Assembly)
            .That()
            .ResideInNamespace(InfrastructureNamespace)
            .ShouldNot()
            .HaveDependencyOn(ContractsNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Contracts_ShouldNotDependOnDomain()
    {
        var result = Types
            .InAssembly(typeof(TaskManager.Contracts.Requests.CreatePermissionRequest).Assembly)
            .ShouldNot()
            .HaveDependencyOn(DomainNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Contracts_ShouldNotDependOnApplication()
    {
        var result = Types
            .InAssembly(typeof(TaskManager.Contracts.Requests.CreatePermissionRequest).Assembly)
            .ShouldNot()
            .HaveDependencyOn(ApplicationNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Contracts_ShouldNotDependOnInfrastructure()
    {
        var result = Types
            .InAssembly(typeof(TaskManager.Contracts.Requests.CreatePermissionRequest).Assembly)
            .ShouldNot()
            .HaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Contracts_ShouldNotDependOnApi()
    {
        var result = Types
            .InAssembly(typeof(TaskManager.Contracts.Requests.CreatePermissionRequest).Assembly)
            .ShouldNot()
            .HaveDependencyOn(ApiNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Domain_ShouldNotUseEntityFramework()
    {
        var result = Types
            .InAssembly(typeof(TaskManager.Domain.Common.BaseEntity).Assembly)
            .ShouldNot()
            .HaveDependencyOn("Microsoft.EntityFrameworkCore")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Domain_ShouldNotUseNpgsql()
    {
        var result = Types
            .InAssembly(typeof(TaskManager.Domain.Common.BaseEntity).Assembly)
            .ShouldNot()
            .HaveDependencyOn("Npgsql")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}
