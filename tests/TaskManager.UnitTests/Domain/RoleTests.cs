using FluentAssertions;
using TaskManager.Domain.Entities;

namespace TaskManager.UnitTests.Domain;

public class RoleTests
{
    [Fact]
    public void Constructor_ShouldSetProperties_WhenValidArguments()
    {
        var role = new Role("Admin", "Administrator role");

        role.Id.Should().NotBeEmpty();
        role.Name.Should().Be("Admin");
        role.Description.Should().Be("Administrator role");
        role.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        role.IsActive.Should().BeTrue();
        role.DeletedAt.Should().BeNull();
    }

    [Fact]
    public void Update_ShouldChangeNameAndDescription()
    {
        var role = new Role("Admin", "Administrator role");

        role.Update("Editor", "Editor role");

        role.Name.Should().Be("Editor");
        role.Description.Should().Be("Editor role");
    }

    [Fact]
    public void Delete_ShouldSetDeletedAt()
    {
        var role = new Role("Admin", "Administrator role");

        role.Delete();

        role.DeletedAt.Should().NotBeNull();
        role.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void AssignPermission_ShouldAddRolePermission_WhenPermissionNotAssigned()
    {
        var role = new Role("Admin", "Administrator role");
        var permissionId = Guid.NewGuid();

        role.AssignPermission(permissionId);

        role.RolePermissions.Should().HaveCount(1);
        var rp = role.RolePermissions.Single();
        rp.RoleId.Should().Be(role.Id);
        rp.PermissionId.Should().Be(permissionId);
    }

    [Fact]
    public void AssignPermission_ShouldThrow_WhenPermissionAlreadyAssigned()
    {
        var role = new Role("Admin", "Administrator role");
        var permissionId = Guid.NewGuid();

        role.AssignPermission(permissionId);

        Action act = () => role.AssignPermission(permissionId);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Permission is already assigned to the role.");
    }

    [Fact]
    public void AssignPermission_ShouldAllowMultiplePermissions()
    {
        var role = new Role("Admin", "Administrator role");
        var permissionId1 = Guid.NewGuid();
        var permissionId2 = Guid.NewGuid();

        role.AssignPermission(permissionId1);
        role.AssignPermission(permissionId2);

        role.RolePermissions.Should().HaveCount(2);
    }

    [Fact]
    public void RolePermissions_ShouldBeEmpty_WhenCreated()
    {
        var role = new Role("Admin", "Administrator role");

        role.RolePermissions.Should().BeEmpty();
    }
}
