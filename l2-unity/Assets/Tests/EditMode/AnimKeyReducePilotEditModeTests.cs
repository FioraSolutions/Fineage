using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Guards the offline keyframe reduction (tools/anim-keyreduce). The probe fixture holds the
/// ORIGINAL curve values (before reduction) at every original key time and segment midpoint for
/// the most animated editor curves of each reduced clip. Unity must load the reduced YAML clip and
/// reproduce those values within the tool tolerance (plus float32 evaluation slack).
/// </summary>
public sealed class AnimKeyReducePilotEditModeTests
{
    private const string ProbeFile = "Tests/EditMode/Fixtures/anim-keyreduce-pilot-probes.json";

    // Unity evaluates curves in float32; the tool validates in float64.
    private const float Float32Slack = 2e-6f;

    [System.Serializable]
    private sealed class ProbeCurve
    {
        public string path;
        public string attribute;
        public float tolerance;
        public float[] times;
        public float[] values;
    }

    [System.Serializable]
    private sealed class ProbeClip
    {
        public string asset;
        public ProbeCurve[] curves;
    }

    [System.Serializable]
    private sealed class ProbeSet
    {
        public ProbeClip[] clips;
    }

    private static ProbeSet LoadProbes()
    {
        string file = Path.Combine(Application.dataPath, ProbeFile);
        Assert.IsTrue(File.Exists(file), $"anim-keyreduce probe fixture missing: {file}");
        var set = JsonUtility.FromJson<ProbeSet>(File.ReadAllText(file));
        Assert.IsNotNull(set?.clips, $"anim-keyreduce probe fixture has no clips: {file}");
        Assert.IsNotEmpty(set.clips, $"anim-keyreduce probe fixture has no clips: {file}");
        return set;
    }

    [Test]
    public void Reduced_clips_load_and_match_original_curves_within_tolerance()
    {
        var failures = new List<string>();
        foreach (ProbeClip probe in LoadProbes().clips)
        {
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(probe.asset);
            if (clip == null)
            {
                failures.Add($"{probe.asset}: AnimationClip failed to load");
                continue;
            }

            foreach (ProbeCurve pc in probe.curves)
            {
                var binding = EditorCurveBinding.FloatCurve(pc.path, typeof(Transform), pc.attribute);
                AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);
                if (curve == null)
                {
                    failures.Add($"{probe.asset}: missing curve {pc.path} {pc.attribute}");
                    continue;
                }

                float maxErr = 0f;
                float atTime = 0f;
                for (int i = 0; i < pc.times.Length; i++)
                {
                    float err = Mathf.Abs(curve.Evaluate(pc.times[i]) - pc.values[i]);
                    if (err > maxErr)
                    {
                        maxErr = err;
                        atTime = pc.times[i];
                    }
                }

                if (maxErr > pc.tolerance + Float32Slack)
                {
                    failures.Add($"{probe.asset}: {pc.path} {pc.attribute} max error {maxErr:G4} at t={atTime:G6} > {pc.tolerance:G4}");
                }
            }
        }

        Assert.IsEmpty(failures, string.Join("\n", failures));
    }
}
