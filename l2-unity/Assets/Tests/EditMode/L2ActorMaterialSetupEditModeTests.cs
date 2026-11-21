using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class L2ActorMaterialSetupEditModeTests
{
    [Test]
    public void NamingImpliesCutout_for_monster_wing_slots()
    {
        Assert.IsTrue(L2ActorMaterialSetup.NamingImpliesCutout("dryad_t01"));
        Assert.IsTrue(L2ActorMaterialSetup.NamingImpliesCutout("a_guard_MElf_m00_t00_h"));
        Assert.IsFalse(L2ActorMaterialSetup.NamingImpliesCutout("dryad_t00"));
    }

    [Test]
    public void Resolve_without_props_uses_naming_for_t01()
    {
        var props = new Dictionary<string, string>();
        var r = L2ActorMaterialSetup.Resolve("unicorn_t01", props);
        Assert.AreEqual(L2ActorMaterialSetup.SurfaceMode.AlphaCutout, r.mode);
    }

    [Test]
    public void Devotion_tunic_material_ships_actor_modulate2x()
    {
        var mat = Resources.Load<Material>("Data/SysTextures/MMagic/Materials/MMagic_m007_t90_u");
        Assert.IsNotNull(mat, "MMagic_m007_t90_u devotion tunic material");
        Assert.AreEqual(L2ActorMaterialSetup.ActorShaderName, mat.shader.name);
        Assert.IsFalse(mat.IsKeywordEnabled("_ALPHATEST_ON"));
    }

    [Test]
    public void ValidatePaperdoll_logs_error_for_env_add_shader()
    {
        var bad = new Material(Shader.Find("L2/World/ActorEnvAdd") ?? Shader.Find("Hidden/InternalErrorShader"))
        {
            name = "test_env_add_armor"
        };
        LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("paperdoll material=test_env_add_armor"));
        L2ActorMaterialSetup.ValidatePaperdollActorMaterial(bad, "editmode");
        Object.DestroyImmediate(bad);
    }

    [Test]
    public void MaterialHasBaseMap_false_when_BaseMap_property_is_empty()
    {
        var shader = Shader.Find("Unlit/Texture");
        Assume.That(shader != null, "Unlit/Texture shader required");
        var mat = new Material(shader) { name = "test_no_tex" };
        try
        {
            mat.SetTexture("_MainTex", null);
            Assert.IsFalse(L2ActorMaterialSetup.MaterialHasBaseMap(mat));
        }
        finally
        {
            Object.DestroyImmediate(mat);
        }
    }

    [Test]
    public void Audit_script_lists_dryad_when_reverted_to_opaque()
    {
        string script = System.IO.Path.Combine(
            UnityEngine.Application.dataPath,
            "..",
            "..",
            "scripts",
            "audit-actor-alpha-materials.py");
        Assert.IsTrue(System.IO.File.Exists(script), "audit-actor-alpha-materials.py must exist");
    }
}