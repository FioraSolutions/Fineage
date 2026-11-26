using NUnit.Framework;

/// <summary>Regression guard for #1467 / #1471 glove thumb bone remap.</summary>
public sealed class SkinnedMeshBoneResolverEditModeTests
{
    [Test]
    public void Blender_misnamed_thumb_maps_to_parent_hand_finger01()
    {
        Assert.IsTrue(SkinnedMeshBoneResolver.TryResolveDuplicateBoneName(
            "Bip01_L_Finger01.001",
            "Bip01_R_Finger0",
            out string bone));
        Assert.AreEqual("Bip01_R_Finger01", bone);
    }

    [Test]
    public void Left_hand_thumb_duplicate_uses_parent_side()
    {
        Assert.IsTrue(SkinnedMeshBoneResolver.TryResolveThumbFromFinger0Parent(
            "Bip01 R Finger01.001",
            "Bip01 L Finger0",
            out string bone));
        Assert.AreEqual("Bip01_L_Finger01", bone);
    }
}