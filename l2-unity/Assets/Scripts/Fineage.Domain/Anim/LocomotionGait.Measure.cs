using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public static partial class LocomotionGait
{
    const int SampleCount = 24;

    public static float MeasureStride(AnimationClip clip, Transform skeleton)
    {
        if (clip == null || skeleton == null || clip.length < 0.01f)
            return 0f;

        long key = CacheKey(clip, skeleton);
        if (StrideCache.TryGetValue(key, out float cached))
            return cached;

        float stride = MeasureStrideUncached(clip, skeleton);
        StrideCache[key] = stride;
        return stride;
    }

    static long CacheKey(AnimationClip clip, Transform skeleton)
    {
        Vector3 s = skeleton.lossyScale;
        int sx = Mathf.RoundToInt(s.x * 100f);
        int sy = Mathf.RoundToInt(s.y * 100f);
        int sz = Mathf.RoundToInt(s.z * 100f);
        return ((long)clip.GetInstanceID() << 32) ^ (uint)(sx * 73856093 ^ sy * 19349663 ^ sz * 83492791);
    }

    static float MeasureStrideUncached(AnimationClip clip, Transform skeleton)
    {
        Transform[] all = skeleton.GetComponentsInChildren<Transform>(true);
        var pos = new Vector3[all.Length];
        var rot = new Quaternion[all.Length];
        var scale = new Vector3[all.Length];
        for (int i = 0; i < all.Length; i++)
        {
            pos[i] = all[i].localPosition;
            rot[i] = all[i].localRotation;
            scale[i] = all[i].localScale;
        }

        Animator animator = skeleton.GetComponentInChildren<Animator>();
        bool animatorWasEnabled = animator != null && animator.enabled;
        if (animator != null)
            animator.enabled = false;

        try
        {
            FindBones(skeleton, out Transform pelvis, out Transform leftFoot, out Transform rightFoot);
            if (pelvis == null)
                pelvis = skeleton;

            float leftAmp = 0f;
            float rightAmp = 0f;
            if (leftFoot != null)
                leftAmp = MeasureFootSwing(clip, skeleton.gameObject, animator, pelvis, leftFoot);
            if (rightFoot != null)
                rightAmp = MeasureFootSwing(clip, skeleton.gameObject, animator, pelvis, rightFoot);

            float stride = leftAmp + rightAmp;
            if (stride >= MinStrideMeters)
                return stride;

            float rootTravel = MeasureRootTravel(clip, skeleton.gameObject, animator, skeleton);
            if (rootTravel >= MinStrideMeters)
                return rootTravel;
            return 0f;
        }
        finally
        {
            for (int i = 0; i < all.Length; i++)
            {
                if (all[i] == null)
                    continue;
                all[i].localPosition = pos[i];
                all[i].localRotation = rot[i];
                all[i].localScale = scale[i];
            }

            if (animator != null)
                animator.enabled = animatorWasEnabled;
        }
    }

    static float MeasureFootSwing(
        AnimationClip clip,
        GameObject go,
        Animator animator,
        Transform pelvis,
        Transform foot)
    {
        float minX = float.MaxValue, maxX = float.MinValue;
        float minZ = float.MaxValue, maxZ = float.MinValue;
        int last = SampleCount - 1;
        for (int i = 0; i <= last; i++)
        {
            float time = clip.length * i / last;
            EvaluateClip(go, clip, time, animator);
            Vector3 rel = pelvis.InverseTransformPoint(foot.position);
            if (rel.x < minX) minX = rel.x;
            if (rel.x > maxX) maxX = rel.x;
            if (rel.z < minZ) minZ = rel.z;
            if (rel.z > maxZ) maxZ = rel.z;
        }

        float rangeX = maxX - minX;
        float rangeZ = maxZ - minZ;
        return rangeZ >= rangeX ? rangeZ : rangeX;
    }

    static float MeasureRootTravel(AnimationClip clip, GameObject go, Animator animator, Transform root)
    {
        EvaluateClip(go, clip, 0f, animator);
        Vector3 start = root.position;
        float max = 0f;
        int last = SampleCount - 1;
        for (int i = 1; i <= last; i++)
        {
            float time = clip.length * i / last;
            EvaluateClip(go, clip, time, animator);
            Vector3 delta = root.position - start;
            delta.y = 0f;
            float dist = delta.magnitude;
            if (dist > max)
                max = dist;
        }

        return max;
    }

    static bool UseTransformSampling(AnimationClip clip, Animator animator)
    {
        if (clip.legacy || animator == null)
            return true;
        Avatar avatar = animator.avatar;
        return avatar == null || !avatar.isValid;
    }

    static void EvaluateClip(GameObject go, AnimationClip clip, float time, Animator animator)
    {
        if (UseTransformSampling(clip, animator))
        {
            clip.SampleAnimation(go, time);
            return;
        }

        PlayableGraph graph = PlayableGraph.Create("LocomotionGait.Measure");
        graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
        bool wasEnabled = animator.enabled;
        animator.enabled = true;
        try
        {
            AnimationClipPlayable playable = AnimationClipPlayable.Create(graph, clip);
            playable.SetApplyFootIK(false);
            AnimationPlayableOutput output = AnimationPlayableOutput.Create(graph, "gait", animator);
            output.SetSourcePlayable(playable);
            playable.SetTime(time);
            graph.Evaluate();
        }
        finally
        {
            if (graph.IsValid())
                graph.Destroy();
            animator.enabled = wasEnabled;
        }
    }
}
