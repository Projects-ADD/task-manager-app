using FluentAssertions;
using TaskManager.Domain.Entities;

namespace TaskManager.UnitTests.Domain;

public class RolePermissionTests
{
    [Fact]
    public void Constructor_ShouldSetProperties_WhenValidArguments()
    {
        var roleId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();

        var rp = new RolePermission(roleId, permissionId);

        rp.Id.Should().NotBeEmpty();
        rp.RoleId.Should().Be(roleId);
        rp.PermissionId.Should().Be(permissionId);
    }

    [Fact]
    public void Constructor_ShouldGenerateUniqueIds_ForDifferentInstances()
    {
        var rp1 = new RolePermission(Guid.NewGuid(), Guid.NewGuid());
        var rp2 = new RolePermission(Guid.NewGuid(), Guid.NewGuid());

        rp1.Id.Should().NotBe(rp2.Id);
    }
}
