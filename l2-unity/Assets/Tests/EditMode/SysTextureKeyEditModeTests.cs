using NUnit.Framework;

public sealed class SysTextureKeyEditModeTests
{
    [Test]
    public void TwoPart_Interlude_token_uses_the_second_segment()
    {
        Assert.IsTrue(SysTextureKey.TrySplit(
            "LineageNPCsTex.a_guard_MElf_m00_t00_b00", out string pkg, out string asset));
        Assert.AreEqual("LineageNPCsTex", pkg);
        Assert.AreEqual("a_guard_MElf_m00_t00_b00", asset);
    }

    [Test]
    public void ThreePart_Classic_token_uses_the_leaf_not_the_group()
    {
        Assert.IsTrue(SysTextureKey.TrySplit(
            "LineageNpcsTex3.a_mageguild_master_MHuman_re.a_mageguild_master_MHuman_re_m00_t00_b00",
            out string pkg, out string asset));
        Assert.AreEqual("LineageNpcsTex3", pkg);
        Assert.AreEqual("a_mageguild_master_MHuman_re_m00_t00_b00", asset);
        Assert.AreNotEqual("a_mageguild_master_MHuman_re", asset);
    }

    [Test]
    public void Rejects_empty_or_bare_names()
    {
        Assert.IsFalse(SysTextureKey.TrySplit(null, out _, out _));
        Assert.IsFalse(SysTextureKey.TrySplit("", out _, out _));
        Assert.IsFalse(SysTextureKey.TrySplit("None", out _, out _));
        Assert.IsFalse(SysTextureKey.TrySplit(".bare", out _, out _));
        Assert.IsFalse(SysTextureKey.TrySplit("pkg.", out _, out _));
    }
}
