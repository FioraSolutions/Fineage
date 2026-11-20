#if UNITY_EDITOR
using NUnit.Framework;

/// <summary>
/// Alpha-cutout world textures (bridge bars, NPC hair cards) must use the DXT5 rule,
/// not BC7 from the opaque world-map/world-character rules (#1520 regression).
/// </summary>
public sealed class WorldAlphaCutoutImportEditModeTests
{
    const string BridgeBar = "Assets/Resources/Data/Textures/giran_port_t/Giran_port_bridgebar.png";
    const string ElfNpcFace = "Assets/Resources/Data/SysTextures/LineageNpcsTex/a_common_people_FElf_m00_t00_f.png";

    [Test]
    public void Bridgebar_matches_world_alpha_cutout_rule()
    {
        var rule = FineageAssetImportRules.ResolveTextureRule(BridgeBar, sourceHasAlpha: true);
        Assert.NotNull(rule);
        Assert.AreEqual("world-alpha-cutout", rule.id);
    }

    [Test]
    public void Opaque_splat_stays_world_map_not_alpha_cutout()
    {
        const string splat = "Assets/Resources/Data/Textures/T_24_12/24_12_splat1.png";
        var rule = FineageAssetImportRules.ResolveTextureRule(splat, sourceHasAlpha: false);
        Assert.NotNull(rule);
        Assert.AreEqual("world-map", rule.id);
    }

    [Test]
    public void Elf_npc_face_with_alpha_uses_cutout_rule()
    {
        var rule = FineageAssetImportRules.ResolveTextureRule(ElfNpcFace, sourceHasAlpha: true);
        Assert.NotNull(rule);
        Assert.AreEqual("world-alpha-cutout", rule.id);
    }
}
#endif