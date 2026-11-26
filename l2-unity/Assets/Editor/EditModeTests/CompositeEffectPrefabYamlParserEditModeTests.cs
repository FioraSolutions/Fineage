using NUnit.Framework;

public sealed class CompositeEffectPrefabYamlParserEditModeTests
{
    [Test]
    public void TryResolveCompositePrefabDiskPath_resolves_dual_soulshot_composite()
    {
        bool ok = CompositeEffectPrefabYamlParser.TryResolveCompositePrefabDiskPath(
            "soul_N_stick_composite_dual",
            out string path,
            out string folder);
        Assert.IsTrue(ok, "dual soulshot composite YAML path");
        Assert.AreEqual("soul_N_stick", folder);
        StringAssert.Contains("soul_N_stick_composite_dual.prefab", path);
    }

    [Test]
    public void TryBuildPartsFromYaml_repairs_dual_weapon_soulshot_parts()
    {
        bool ok = CompositeEffectPrefabYamlParser.TryBuildPartsFromYaml(
            "soul_N_stick_composite_dual",
            out CompositePart[] parts);
        Assert.IsTrue(ok);
        Assert.NotNull(parts);
        Assert.GreaterOrEqual(parts.Length, 2, "dual charge needs RH+LH weapon transient parts");
    }
}