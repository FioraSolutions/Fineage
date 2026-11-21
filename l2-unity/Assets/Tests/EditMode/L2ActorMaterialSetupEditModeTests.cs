using System.Collections.Generic;
using NUnit.Framework;

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