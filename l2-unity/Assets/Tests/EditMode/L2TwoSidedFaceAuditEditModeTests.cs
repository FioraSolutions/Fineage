using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Fineage.TerrainBsp;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed class L2TwoSidedFaceAuditEditModeTests
{
    static string DataRoot =>
        Path.GetFullPath(Path.Combine(Application.dataPath, "Resources", "Data"));

    [Test]
    public void Original_TwoSided_clears_backface_cull()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        Assert.IsNotNull(shader, "URP Lit shader missing");
        var mat = new Material(shader) { name = "GL_CA_king10_sh" };
        mat.SetFloat("_Cull", 2f);
        L2TwoSidedMaterials.BindSharedMaterial(mat, logIfPropsMissing: true, "test");
        Assert.AreEqual(L2TwoSidedMaterials.CullOff, mat.GetFloat("_Cull"), 0.01f);
        Assert.IsTrue(mat.doubleSidedGI);
        Object.DestroyImmediate(mat);
    }

    [Test]
    public void Missing_weapon_props_are_logged_and_not_guessed()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        Assert.IsNotNull(shader, "URP Lit shader missing");
        L2TwoSidedMaterials.EnsureLoaded();
        var mat = new Material(shader) { name = "no_such_weapon_material_f588" };
        mat.SetFloat("_Cull", 2f);
        LogAssert.Expect(LogType.Error, new Regex("no_such_weapon_material_f588 props missing"));
        L2TwoSidedMaterials.BindSharedMaterial(mat, logIfPropsMissing: true, "weapon attach");
        Assert.AreEqual(2f, mat.GetFloat("_Cull"), 0.01f);
        Object.DestroyImmediate(mat);
    }

    [Test]
    public void Subtract_does_not_carve_an_add_that_follows_it()
    {
        L2BspBrush wall = Wall(new Vector3(0f, 0f, 1f));
        L2BspBrush hole = Hole(new Vector3(0f, 1f, 1f));

        var wallAfterHole = new List<L2TileFaceAudit.Record>();
        L2TileFaceAudit.AuditBrushes("order", new[] { hole, wall }, wallAfterHole);
        Assert.IsFalse(wallAfterHole.Exists(r => r.kind == "carved-away"),
            "a subtract does not cut an additive brush placed after it");

        var wallBeforeHole = new List<L2TileFaceAudit.Record>();
        L2TileFaceAudit.AuditBrushes("order", new[] { wall, hole }, wallBeforeHole);
        Assert.IsTrue(wallBeforeHole.Exists(r => r.kind == "carved-away"));
    }

    [Test]
    public void Gludio_audit_lists_two_sided_faces_still_culled()
    {
        Assert.IsTrue(Directory.Exists(DataRoot), DataRoot);
        List<L2TileFaceAudit.Record> rows = L2TileFaceAudit.AuditTile(DataRoot, "19_21");
        Assert.IsFalse(rows.Exists(r => r.kind == "missing-mesh"),
            "19_21 static mesh ref is missing from the importer path");
        Assert.IsTrue(rows.Exists(r => r.kind == "culled-two-sided" && r.subject == "GL_CA_king10_sh"),
            "GL_CA_king10_sh is TwoSided in castle_kent props and still Cull Back");
    }

    static L2BspBrush Wall(Vector3 position)
    {
        return new L2BspBrush
        {
            name = "Wall",
            csgOper = L2BspCsgCarver.CsgAdd,
            position = position,
            prePivot = Vector3.zero,
            polyFlags = new[] { "PF_Default" },
            polygons = new[]
            {
                new L2BspPoly
                {
                    texture = "castle_kent_t.GL_CA_king10_sh",
                    polyFlags = new[] { "PF_Default" },
                    origin = Vector3.zero,
                    textureU = Vector3.right,
                    textureV = Vector3.forward,
                    vertices = new[]
                    {
                        new Vector3(0f, 0f, 0f),
                        new Vector3(512f, 0f, 0f),
                        new Vector3(512f, 0f, 256f),
                        new Vector3(0f, 0f, 256f),
                    }
                }
            }
        };
    }

    static L2BspBrush Hole(Vector3 position)
    {
        Vector3 min = new Vector3(-10f, -20f, -10f) - position;
        Vector3 max = new Vector3(600f, 20f, 400f) - position;
        return new L2BspBrush
        {
            name = "Hole",
            csgOper = L2BspCsgCarver.CsgSubtract,
            position = position,
            prePivot = Vector3.zero,
            polyFlags = new[] { "PF_Default" },
            polygons = new[]
            {
                new L2BspPoly
                {
                    texture = "Engine.DefaultTexture",
                    polyFlags = new[] { "PF_Default" },
                    vertices = new[]
                    {
                        new Vector3(min.x, min.y, min.z),
                        new Vector3(max.x, min.y, min.z),
                        new Vector3(max.x, max.y, min.z),
                        new Vector3(min.x, max.y, max.z),
                        new Vector3(max.x, max.y, max.z),
                        new Vector3(min.x, min.y, max.z),
                    }
                }
            }
        };
    }
}
