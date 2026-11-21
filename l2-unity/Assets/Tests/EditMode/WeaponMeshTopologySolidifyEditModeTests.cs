using NUnit.Framework;
using UnityEngine;

public sealed class WeaponMeshTopologySolidifyEditModeTests
{
    [Test]
    public void Solidify_repairs_single_sided_plane()
    {
        var mesh = new Mesh { name = "plane_test" };
        mesh.vertices = new[]
        {
            new Vector3(0f, 0f, 0f),
            new Vector3(1f, 0f, 0f),
            new Vector3(0.5f, 1f, 0f),
        };
        mesh.triangles = new[] { 0, 1, 2 };
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        WeaponMeshTopology.Report before = WeaponMeshTopology.Analyze(mesh, mesh.name);
        Assert.IsTrue(before.IsDegenerate);

        Mesh fixedMesh = WeaponMeshTopology.BuildSolidifiedMesh(mesh);
        Assert.IsNotNull(fixedMesh);
        WeaponMeshTopology.Report after = WeaponMeshTopology.Analyze(fixedMesh, mesh.name);
        Assert.IsFalse(after.IsDegenerate, after.FailureReason);
    }
}