namespace Fineage.Client.Domain.Tests;

public sealed class SkinnedMeshBoneResolverTests
{
    [Fact]
    public void Resolves_blender_l_thumb_under_r_finger0_to_r_finger01()
    {
        Assert.True(SkinnedMeshBoneResolver.TryResolveDuplicateBoneName(
            "Bip01_L_Finger01.001",
            "Bip01_R_Finger0",
            out string bone));
        Assert.Equal("Bip01_R_Finger01", bone);
    }

    [Fact]
    public void Resolves_spaced_blender_thumb_to_underscore_skeleton_name()
    {
        Assert.True(SkinnedMeshBoneResolver.TryResolveDuplicateBoneName(
            "Bip01 L Finger01.001",
            "Bip01 R Finger0",
            out string bone));
        Assert.Equal("Bip01_R_Finger01", bone);
    }

    [Fact]
    public void Resolves_left_thumb_under_left_finger0_from_parent_side()
    {
        Assert.True(SkinnedMeshBoneResolver.TryResolveThumbFromFinger0Parent(
            "Bip01_R_Finger01.001",
            "Bip01_L_Finger0",
            out string bone));
        Assert.Equal("Bip01_L_Finger01", bone);
    }
}