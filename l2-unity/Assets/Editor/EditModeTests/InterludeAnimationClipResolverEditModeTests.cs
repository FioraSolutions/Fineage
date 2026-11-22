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
        string query = InterludeAnimationClipResolver.BuildPlayerSpAtkQuery(3, "_dual", "MFighter");
        Assert.IsTrue(
            AnimTweenTable.Instance.TryResolveExactSequence("MFighter_anim", query, out string seq));
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
    public void MFighter_spatk03_dual_loads_from_the_catalog_take()
    {
        string query = InterludeAnimationClipResolver.BuildPlayerSpAtkQuery(3, "_dual", "MFighter");
        Assert.IsTrue(
            AnimTweenTable.Instance.TryResolveExactSequence("MFighter_anim", query, out string seq));
        string take = InterludeAnimResourceTakeTable.ResourcesTakeName("MFighter_anim", seq);
        Assert.AreEqual("SpAtk03_Dual_MFighter", take);
        const string meshStem = "MFighter_m000_b";
        string path = InterludeAnimationClipResolver.BuildResourcesPath(
            "Data/Animations/Fighter/MFighter/Clips/",
            meshStem,
            take);
        AnimationClip clip = InterludeAnimationClipResolver.LoadExact(path);
        Assert.IsNotNull(clip, path);
        Assert.That(clip.name, Does.Contain(take));
    }
}