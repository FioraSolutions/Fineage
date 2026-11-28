using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

/// <summary>
/// Last node of the shared motion graph. Copies the animated pose and writes each bone's
/// bind scale back, so a clip's scale curves cannot inflate the mesh. Root motion velocity is
/// forwarded only when the Animator applies root motion.
/// </summary>
public sealed class MotionBoneScale : IDisposable
{
    readonly AnimationScriptPlayable _playable;
    NativeArray<TransformStreamHandle> _bones;
    NativeArray<Vector3> _scales;

    MotionBoneScale(
        AnimationScriptPlayable playable,
        NativeArray<TransformStreamHandle> bones,
        NativeArray<Vector3> scales)
    {
        _playable = playable;
        _bones = bones;
        _scales = scales;
    }

    public AnimationScriptPlayable Playable => _playable;

    public static void BindBones(Animator animator, List<TransformStreamHandle> handles, List<Vector3> scales)
    {
        if (animator == null)
        {
            throw new InvalidOperationException(
                "[MotionPipeline] Animator missing — bone scale lock was not created.");
        }

        if (animator.avatar == null || !animator.avatar.isValid)
        {
            throw new InvalidOperationException(
                "[MotionPipeline] Animator '" + animator.name +
                "' has no Avatar — bone scale lock was not created, so a clip would inflate the mesh.");
        }

        Transform[] transforms = animator.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < transforms.Length; i++)
        {
            Transform bone = transforms[i];
            if (bone == null || bone == animator.transform)
                continue;
            handles.Add(animator.BindStreamTransform(bone));
            scales.Add(bone.localScale);
        }

        if (handles.Count == 0)
        {
            throw new InvalidOperationException(
                "[MotionPipeline] Animator '" + animator.name +
                "' has no bones — bone scale lock was not created, so a clip would inflate the mesh.");
        }
    }

    public static MotionBoneScale Create(
        PlayableGraph graph,
        List<TransformStreamHandle> handles,
        List<Vector3> scales,
        bool forwardRootMotion)
    {
        if (!graph.IsValid())
        {
            throw new InvalidOperationException(
                "[MotionPipeline] motion graph is not valid — bone scale lock was not created.");
        }

        if (handles == null || scales == null || handles.Count == 0 || handles.Count != scales.Count)
        {
            throw new InvalidOperationException(
                "[MotionPipeline] bone scale lock has no bones — a clip would inflate the mesh.");
        }

        var bones = new NativeArray<TransformStreamHandle>(handles.ToArray(), Allocator.Persistent);
        var bindScales = new NativeArray<Vector3>(scales.ToArray(), Allocator.Persistent);
        var job = new Job
        {
            bones = bones,
            scales = bindScales,
            forwardRootMotion = forwardRootMotion,
        };
        AnimationScriptPlayable playable = AnimationScriptPlayable.Create(graph, job, 1);
        playable.SetInputWeight(0, 1f);
        return new MotionBoneScale(playable, bones, bindScales);
    }

    public void Dispose()
    {
        if (_bones.IsCreated)
            _bones.Dispose();
        if (_scales.IsCreated)
            _scales.Dispose();
        _bones = default;
        _scales = default;
    }

    struct Job : IAnimationJob
    {
        public NativeArray<TransformStreamHandle> bones;
        public NativeArray<Vector3> scales;

        // Unity logs "Root Motion is OFF" on every frame when a job reads or writes the stream
        // velocity while the Animator has applyRootMotion disabled, so the velocity is only
        // forwarded when the Animator actually applies root motion.
        public bool forwardRootMotion;

        public void ProcessRootMotion(AnimationStream stream)
        {
            if (!forwardRootMotion || stream.inputStreamCount < 1)
                return;
            AnimationStream input = stream.GetInputStream(0);
            stream.velocity = input.velocity;
            stream.angularVelocity = input.angularVelocity;
        }

        public void ProcessAnimation(AnimationStream stream)
        {
            if (stream.inputStreamCount < 1)
                return;
            AnimationStream input = stream.GetInputStream(0);
            for (int i = 0; i < bones.Length; i++)
            {
                TransformStreamHandle handle = bones[i];
                if (!handle.IsValid(stream) || !handle.IsValid(input))
                    continue;
                handle.SetLocalPosition(stream, handle.GetLocalPosition(input));
                handle.SetLocalRotation(stream, handle.GetLocalRotation(input));
                handle.SetLocalScale(stream, scales[i]);
            }
        }
    }
}
