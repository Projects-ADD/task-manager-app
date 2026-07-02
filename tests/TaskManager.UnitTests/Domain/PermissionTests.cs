using FluentAssertions;
using TaskManager.Domain.Entities;

namespace TaskManager.UnitTests.Domain;

public class PermissionTests
{
    [Fact]
    public void Constructor_ShouldSetProperties_WhenValidArguments()
    {
        var permission = new Permission("tasks.read", "Read tasks");

        permission.Id.Should().NotBeEmpty();
        permission.Name.Should().Be("tasks.read");
        permission.Description.Should().Be("Read tasks");
        permission.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        permission.IsActive.Should().BeTrue();
        permission.DeletedAt.Should().BeNull();
    }

    [Fact]
    public void Update_ShouldChangeNameAndDescription()
    {
        var permission = new Permission("tasks.read", "Read tasks");

        permission.Update("tasks.write", "Write tasks");

        permission.Name.Should().Be("tasks.write");
        permission.Description.Should().Be("Write tasks");
    }

    [Fact]
    public void Delete_ShouldSetDeletedAt()
    {
        var permission = new Permission("tasks.read", "Read tasks");

        permission.Delete();

        permission.DeletedAt.Should().NotBeNull();
        permission.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        permission.IsActive.Should().BeTrue();
    }

    [Fact]
    public void RolePermissions_ShouldBeEmpty_WhenCreated()
    {
        var permission = new Permission("tasks.read", "Read tasks");

        permission.RolePermissions.Should().BeEmpty();
    }

    [Fact]
    public void RolePermissions_ShouldBeInitialized_AsNewList()
    {
        var permission = new Permission("tasks.read", "Read tasks");

        permission.RolePermissions.Should().NotBeNull();
    }
}
