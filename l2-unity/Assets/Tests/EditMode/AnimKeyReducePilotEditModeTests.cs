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
[Category("Slow")]
public sealed class AnimKeyReducePilotEditModeTests
{
    private static readonly string[] ProbeFiles =
    {
        "Tests/EditMode/Fixtures/anim-keyreduce-pilot-probes.json",
        "Tests/EditMode/Fixtures/anim-keyreduce-lineagemonsters-probes.json",
        "Tests/EditMode/Fixtures/anim-keyreduce-lineagemonsters2-probes.json",
        "Tests/EditMode/Fixtures/anim-keyreduce-lineagenpcs-batch1-probes.json",
        "Tests/EditMode/Fixtures/anim-keyreduce-lineagenpcs-batch2-probes.json",
        "Tests/EditMode/Fixtures/anim-keyreduce-lineagenpcs-batch3-probes.json",
        "Tests/EditMode/Fixtures/anim-keyreduce-lineagenpcs-batch4-probes.json",
        "Tests/EditMode/Fixtures/anim-keyreduce-lineagenpcs4-probes.json",
        "Tests/EditMode/Fixtures/anim-keyreduce-orc-probes.json",
        "Tests/EditMode/Fixtures/anim-keyreduce-fighter-probes.json",
        "Tests/EditMode/Fixtures/anim-keyreduce-darkelf-probes.json",
        "Tests/EditMode/Fixtures/anim-keyreduce-dwarf-probes.json",
        "Tests/EditMode/Fixtures/anim-keyreduce-elf-probes.json",
        "Tests/EditMode/Fixtures/anim-keyreduce-magic-probes.json",
    };

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

    private static ProbeSet LoadProbes(string relativeToAssets)
    {
        string file = Path.Combine(Application.dataPath, relativeToAssets);
        Assert.IsTrue(File.Exists(file), $"anim-keyreduce probe fixture missing: {file}");
        var set = JsonUtility.FromJson<ProbeSet>(File.ReadAllText(file));
        Assert.IsNotNull(set?.clips, $"anim-keyreduce probe fixture has no clips: {file}");
        Assert.IsNotEmpty(set.clips, $"anim-keyreduce probe fixture has no clips: {file}");
        return set;
    }

    static bool TryGetCurve(AnimationClip clip, string path, string attribute, out AnimationCurve curve)
    {
        foreach (EditorCurveBinding binding in AnimationUtility.GetCurveBindings(clip))
        {
            if (binding.path != path || binding.propertyName != attribute)
                continue;
            curve = AnimationUtility.GetEditorCurve(clip, binding);
            return curve != null;
        }

        curve = null;
        return false;
    }

    [Test]
    public void Reduced_clips_load_and_match_original_curves_within_tolerance()
    {
        var failures = new List<string>();
        var checkedCurves = 0;
        foreach (string probeFile in ProbeFiles)
        {
            foreach (ProbeClip probe in LoadProbes(probeFile).clips)
            {
                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(probe.asset);
                if (clip == null)
                {
                    failures.Add($"{probe.asset}: AnimationClip failed to load");
                    continue;
                }

                foreach (ProbeCurve pc in probe.curves)
                {
                    if (!TryGetCurve(clip, pc.path, pc.attribute, out AnimationCurve curve))
                        continue;

                    checkedCurves++;
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
        }

        Assert.Greater(checkedCurves, 0, "anim-keyreduce probe: no Unity-readable curves were checked");
        Assert.IsEmpty(failures, string.Join("\n", failures));
    }
}