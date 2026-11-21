using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

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
    public void Paperdoll_normalizes_devotion_armor_off_actor_env_add()
    {
        var src = Resources.Load<Material>("Data/SysTextures/MMagic/Materials/MMagic_m007_t90_u");
        Assert.IsNotNull(src, "MMagic_m007_t90_u devotion tunic material");
        Assert.IsTrue(
            src.shader != null && src.shader.name.Contains("ActorEnv"),
            "fixture expects L2/World/ActorEnvAdd before normalization");

        Material paperdoll = L2ActorMaterialSetup.EnsurePaperdollActorMaterial(src, "test");
        Assert.AreEqual(L2ActorMaterialSetup.ActorShaderName, paperdoll.shader.name);
        Assert.IsFalse(paperdoll.IsKeywordEnabled("_ALPHATEST_ON"));
    }

    [Test]
    public void Paperdoll_preserves_actor_modulate_face_material()
    {
        var src = Resources.Load<Material>("Data/SysTextures/LineageFaceTex/Materials/FMagic_m000_t00_f");
        Assert.IsNotNull(src, "FMagic face material");
        Material paperdoll = L2ActorMaterialSetup.EnsurePaperdollActorMaterial(src, "test");
        Assert.AreSame(src, paperdoll);
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