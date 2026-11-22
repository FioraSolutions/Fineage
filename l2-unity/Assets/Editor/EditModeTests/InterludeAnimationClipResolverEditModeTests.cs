using NUnit.Framework;
using UnityEngine;

public sealed class InterludeAnimationClipResolverEditModeTests
{
    [OneTimeSetUp]
    public void LoadTables()
    {
        AnimTweenTable.Instance.Initialize();
        InterludeAnimResourceTakeTable.EnsureLoaded();
    }

    [Test]
    public void AnimTween_resolves_MFighter_dual_spatk03_query()
    {
        Assert.IsTrue(
            AnimTweenTable.Instance.TryResolveExactSequence(
                "MFighter_anim",
                "spatk03_dual_mfighter",
                out string seq));
        Assert.AreEqual("spatk03_dual_MFighter", seq);
    }

    [Test]
    public void Resource_take_maps_FFighter_spatk03_dual_to_shipped_clip_name()
    {
        string take = InterludeAnimResourceTakeTable.ResourcesTakeName(
            "FFighter_anim",
            "spatk03_dual_FFighter");
        Assert.AreEqual("SpAtk03_Dual_FFighter", take);

        var clip = InterludeAnimationClipResolver.LoadExact(
            "Data/Animations/Fighter/FFighter/Clips/FFighter_m000_b.ao_" + take);
        Assert.IsNotNull(clip, "FFighter SpAtk03 dual clip");
    }

    [Test]
    public void MFighter_spatk03_dual_clip_missing_logs_concrete_path()
    {
        var loaded = InterludeAnimationClipResolver.TryLoadPlayerSpAtkCombat(
            "MFighter",
            3,
            "_dual",
            "SpAtk03_dual");
        Assert.IsNull(loaded);
    }
}