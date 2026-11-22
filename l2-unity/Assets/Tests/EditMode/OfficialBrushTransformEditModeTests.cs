using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class OfficialBrushTransformEditModeTests
{
    [Test]
    public void ToWorld_subtracts_prepivot_then_adds_location()
    {
        // 21_23 WaterVolume1 top-Z: local 500, prepivot z=-839.989746, location z=-5120
        // -> 500 - (-839.989746) + (-5120) = -3780.010254 (matches the official FBox max.z)
        var local = new Vector3(16384f, 10500f, 500f);
        var location = new Vector3(32768f, 181248f, -5120f);
        var prepivot = new Vector3(-16384f, -4859.98438f, -839.989746f);
        Vector3 world = OfficialBrushTransform.ToWorld(
            local, location, prepivot, Vector3.one, Vector3.one, Vector3.zero);
        Assert.AreEqual(32768f + 16384f - (-16384f), world.x, 0.01f);
        Assert.AreEqual(-3780.010254f, world.z, 0.01f);
    }

    [Test]
    public void IsTopFace_accepts_upward_UE_Z_normals()
    {
        Assert.IsTrue(OfficialBrushTransform.IsTopFace(new Vector3(0f, 0f, 1f)));
        Assert.IsFalse(OfficialBrushTransform.IsTopFace(new Vector3(0f, 0f, -1f)));
        Assert.IsFalse(OfficialBrushTransform.IsTopFace(new Vector3(1f, 0f, 0f)));
    }

    [Test]
    public void AppendPolygon_fans_a_quad_into_two_triangles()
    {
        var poly = new[]
        {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(1, 1, 0),
            new Vector3(0, 1, 0)
        };
        var verts = new List<Vector3>();
        var tris = new List<int>();
        Assert.IsTrue(OfficialBrushTransform.AppendPolygon(poly, verts, tris));
        Assert.AreEqual(4, verts.Count);
        Assert.AreEqual(6, tris.Count);
        Assert.AreEqual(0, tris[0]);
        Assert.AreEqual(1, tris[1]);
        Assert.AreEqual(2, tris[2]);
        Assert.AreEqual(0, tris[3]);
        Assert.AreEqual(2, tris[4]);
        Assert.AreEqual(3, tris[5]);
    }
}
