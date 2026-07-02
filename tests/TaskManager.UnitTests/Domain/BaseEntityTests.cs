using FluentAssertions;
using TaskManager.Domain.Common;
using TaskManager.Domain.Entities;

namespace TaskManager.UnitTests.Domain;

public class BaseEntityTests
{
    [Fact]
    public void BaseEntity_ShouldSetDefaults_WhenConstructed()
    {
        var permission = new Permission("test", "test");

        permission.Id.Should().NotBeEmpty();
        permission.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        permission.IsActive.Should().BeTrue();
        permission.DeletedAt.Should().BeNull();
        permission.DeletedBy.Should().BeEmpty();
    }

    [Fact]
    public void AggregateRoot_ShouldInheritFromBaseEntity()
    {
        typeof(AggregateRoot).IsSubclassOf(typeof(BaseEntity)).Should().BeTrue();
    }

    [Fact]
    public void Permission_ShouldBeAggregateRoot()
    {
        typeof(Permission).IsSubclassOf(typeof(AggregateRoot)).Should().BeTrue();
    }

    [Fact]
    public void Role_ShouldBeAggregateRoot()
    {
        typeof(Role).IsSubclassOf(typeof(AggregateRoot)).Should().BeTrue();
    }
}
