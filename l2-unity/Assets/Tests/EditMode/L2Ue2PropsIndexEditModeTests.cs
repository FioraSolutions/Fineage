using System;
using System.Collections.Generic;
using NUnit.Framework;

public class L2Ue2PropsIndexEditModeTests
{
    [Test]
    public void Qualified_keys_keep_both_TexPanner0_exports()
    {
        List<L2Ue2PropsIndex.Entry> files = L2Ue2PropsIndex.Load(L2Ue2PropsIndex.TexturesRoot());
        Assert.Greater(files.Count, 0);
        var props = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var sources = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var conflicts = new List<string>();
        L2Ue2PropsIndex.AddKeys(files, props, sources, conflicts);

        Assert.IsTrue(props.TryGetValue("Shaders.TexPanner0", out string shaders));
        StringAssert.Contains("Textures.WhiteCloud", shaders);
        Assert.IsTrue(sources["Shaders.TexPanner0"].Replace('\\', '/').Contains("TexPanner/Shaders/TexPanner0.props.txt"));

        Assert.IsTrue(props.TryGetValue("moon.TexPanner0", out string moon));
        StringAssert.Contains("OpenEyeMoon_myst_bitmap4", moon);
        Assert.IsTrue(sources["moon.TexPanner0"].Replace('\\', '/').Contains("TexPanner/moon/TexPanner0.props.txt"));

        Assert.IsFalse(props.ContainsKey("TexPanner0"), "short name must not win when the exports differ");

        Assert.IsTrue(props.TryGetValue("WaterSurfaceShaderSet.TexOscillator0", out string water));
        StringAssert.Contains("ocean028", water);

        string scoped = "l2_skies" + L2Ue2PropsIndex.PackageScope + "TexOscillator0";
        Assert.IsTrue(props.TryGetValue(scoped, out string skiesOsc));
        StringAssert.Contains("moon.Rotator", skiesOsc);
    }

    [Test]
    public void Shared_material_name_follows_the_package_the_texture_references()
    {
        List<L2Ue2PropsIndex.Entry> files = L2Ue2PropsIndex.Load(L2Ue2PropsIndex.TexturesRoot());
        var grass = files.FindAll(file => file.Name == "Darkelf_grass001_1");
        Assert.Greater(grass.Count, 1);
        L2Ue2PropsIndex.Entry chosen = L2Ue2PropsIndex.Choose(
            grass, "Darkelf_grass001", null, null, out string reason);
        Assert.NotNull(chosen);
        Assert.AreEqual("Shader", chosen.ClassFolder);
        StringAssert.Contains("AlphaTest = true", chosen.Text);
        StringAssert.Contains("Shader", reason);

        var arms = files.FindAll(file => file.Name == "z_arm37");
        L2Ue2PropsIndex.Entry a = L2Ue2PropsIndex.Choose(arms, "interior_A_arm37", null, null, out _);
        L2Ue2PropsIndex.Entry b = L2Ue2PropsIndex.Choose(arms, "interior_B_arm37", null, null, out _);
        Assert.NotNull(a);
        Assert.NotNull(b);
        StringAssert.StartsWith("interior_a_t/", a.RelativePath.Replace('\\', '/'));
        StringAssert.StartsWith("interior_B_t/", b.RelativePath.Replace('\\', '/'));

        L2Ue2PropsIndex.Entry neither = L2Ue2PropsIndex.Choose(arms, null, null, null, out string why);
        Assert.IsNull(neither);
        Assert.AreEqual("ambiguous", why);
    }
}
