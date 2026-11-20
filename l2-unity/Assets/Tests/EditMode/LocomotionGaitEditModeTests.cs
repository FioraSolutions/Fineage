using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Playback rate is unitySpeed * clipLength / stride. Stride is both feet's
/// swing relative to the pelvis (in-place clips) so every mesh shares one formula.
/// </summary>
public sealed class LocomotionGaitEditModeTests
{
    [TearDown]
    public void TearDown()
    {
        LocomotionGait.ClearStrideCache();
    }

    [Test]
    public void Playback_rate_is_speed_times_length_over_stride()
    {
        float rate = LocomotionGait.PlaybackRate(2.268f, 0.67f, 2.70f, 0.2f, 4f);
        Assert.AreEqual(2.268f * 0.67f / 2.70f, rate, 0.001f);
    }

    [Test]
    public void Slower_planar_speed_lowers_playback_rate_with_the_same_stride()
    {
        const float clipLength = 0.67f;
        const float stride = 2.70f;
        float full = LocomotionGait.PlaybackRate(NumberUtils.ScaleToUnity(120f), clipLength, stride, 0.2f, 4f);
        float slowed = LocomotionGait.PlaybackRate(NumberUtils.ScaleToUnity(60f), clipLength, stride, 0.2f, 4f);
        Assert.Less(slowed, full);
        Assert.AreEqual(full * 0.5f, slowed, 0.001f);
    }

    [Test]
    public void Playback_rate_clamps_to_the_requested_range()
    {
        // 10 * 2 / 0.1 = 200 → max 4. 0.2 * 0.2 / 2 = 0.02 → min 0.2.
        Assert.AreEqual(4f, LocomotionGait.PlaybackRate(10f, 2f, 0.1f, 0.2f, 4f), 0.001f);
        Assert.AreEqual(0.2f, LocomotionGait.PlaybackRate(0.2f, 0.2f, 2f, 0.2f, 4f), 0.001f);
    }

    [Test]
    public void Missing_clip_or_speed_returns_a_neutral_clamped_rate()
    {
        Assert.AreEqual(1f, LocomotionGait.PlaybackRate(0f, 1f, 2f, 0.2f, 4f), 0.001f);
        Assert.AreEqual(1f, LocomotionGait.RateForClip(null, null, 2f, 0.2f, 4f), 0.001f);
    }

    [Test]
    public void Unmeasurable_stride_uses_the_generic_fallback_stride()
    {
        float rate = LocomotionGait.PlaybackRate(3f, 1f, 0f, 0.2f, 4f);
        Assert.AreEqual(3f / LocomotionGait.FallbackStrideMeters, rate, 0.001f);
    }

    [Test]
    public void MeasureStride_adds_both_feet_swings_in_pelvis_space()
    {
        GameObject root = new GameObject("GaitRoot");
        try
        {
            Transform pelvis = NewChild(root.transform, "Pelvis", Vector3.zero);
            NewChild(pelvis, "Bip01 L Foot", new Vector3(-0.2f, 0f, 0f));
            NewChild(pelvis, "Bip01 R Foot", new Vector3(0.2f, 0f, 0f));

            var clip = new AnimationClip { name = "run_hand", legacy = true, frameRate = 24f };
            // One cycle: left foot -0.5..+0.5, right foot +0.5..-0.5 (each swing 1m).
            clip.SetCurve("Pelvis/Bip01 L Foot", typeof(Transform), "localPosition.z",
                AnimationCurve.Linear(0f, -0.5f, 1f, 0.5f));
            clip.SetCurve("Pelvis/Bip01 R Foot", typeof(Transform), "localPosition.z",
                AnimationCurve.Linear(0f, 0.5f, 1f, -0.5f));
            clip.SetCurve("Pelvis/Bip01 L Foot", typeof(Transform), "localPosition.x",
                AnimationCurve.Linear(0f, -0.2f, 1f, -0.2f));
            clip.SetCurve("Pelvis/Bip01 R Foot", typeof(Transform), "localPosition.x",
                AnimationCurve.Linear(0f, 0.2f, 1f, 0.2f));

            float stride = LocomotionGait.MeasureStride(clip, root.transform);
            Assert.AreEqual(2f, stride, 0.05f);

            // Pose is restored after sampling.
            Assert.AreEqual(0f, pelvis.Find("Bip01 L Foot").localPosition.z, 0.001f);
        }
        finally
        {
            Object.DestroyImmediate(root);
        }
    }

    [Test]
    public void RateForClip_matches_the_measured_stride_formula()
    {
        GameObject root = new GameObject("GaitRoot");
        try
        {
            Transform pelvis = NewChild(root.transform, "Pelvis", Vector3.zero);
            NewChild(pelvis, "Bip01 L Foot", new Vector3(-0.2f, 0f, 0f));
            NewChild(pelvis, "Bip01 R Foot", new Vector3(0.2f, 0f, 0f));

            var clip = new AnimationClip { name = "walk_hand", legacy = true, frameRate = 24f };
            clip.SetCurve("Pelvis/Bip01 L Foot", typeof(Transform), "localPosition.z",
                AnimationCurve.Linear(0f, -0.4f, 0.8f, 0.4f));
            clip.SetCurve("Pelvis/Bip01 R Foot", typeof(Transform), "localPosition.z",
                AnimationCurve.Linear(0f, 0.4f, 0.8f, -0.4f));

            float stride = LocomotionGait.MeasureStride(clip, root.transform);
            float unity = NumberUtils.ScaleToUnity(120f);
            float rate = LocomotionGait.RateForClip(clip, root.transform, unity, LocomotionGait.MinRunRate, LocomotionGait.MaxRate);
            Assert.AreEqual(LocomotionGait.PlaybackRate(unity, clip.length, stride, LocomotionGait.MinRunRate, LocomotionGait.MaxRate), rate, 0.001f);
        }
        finally
        {
            Object.DestroyImmediate(root);
        }
    }

    static Transform NewChild(Transform parent, string name, Vector3 local)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.transform.localPosition = local;
        return go.transform;
    }
}
