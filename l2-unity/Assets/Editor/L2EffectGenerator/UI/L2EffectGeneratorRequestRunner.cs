#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Polls for Temp/l2-effect-generate-prominence.request while the Editor
/// already holds the project lock (batchmode cannot attach).
/// </summary>
[InitializeOnLoad]
public static class L2EffectGeneratorRequestRunner
{
    const string RequestRelative = "Temp/l2-effect-generate-prominence.request";
    const string DoneRelative = "Temp/l2-effect-generate-prominence.done";
    const string BusyRelative = "Temp/l2-effect-generate-prominence.busy";

    static double _nextPoll;

    static L2EffectGeneratorRequestRunner()
    {
        EditorApplication.update += Poll;
    }

    static void Poll()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
        {
            return;
        }

        double now = EditorApplication.timeSinceStartup;
        if (now < _nextPoll)
        {
            return;
        }

        _nextPoll = now + 0.5d;

        string projectRoot = Directory.GetParent(Application.dataPath)!.FullName;
        string request = Path.Combine(projectRoot, RequestRelative);
        string busy = Path.Combine(projectRoot, BusyRelative);
        if (!File.Exists(request) || File.Exists(busy))
        {
            return;
        }

        try
        {
            File.Delete(request);
            File.WriteAllText(busy, "1");
        }
        catch
        {
            return;
        }

        try
        {
            AssetDatabase.Refresh();
            L2EffectGeneratorFolderBuilder.BuildResult result =
                L2EffectGeneratorFolderBuilder.CreateFromFolder(
                    "Assets/Resources/Data/Effects/el_prominence",
                    "el_prominence",
                    1230,
                    true);

            for (int i = 0; i < result.LogLines.Count; i++)
            {
                Debug.Log("[L2EffectGenerator] " + result.LogLines[i]);
            }

            string done = Path.Combine(projectRoot, DoneRelative);
            File.WriteAllText(
                done,
                result.Success
                    ? "ok\n" + string.Join("\n", result.LogLines)
                    : "fail\n" + result.ErrorMessage + "\n" +
                      string.Join("\n", result.LogLines));

            if (!result.Success)
            {
                Debug.LogError("[L2EffectGenerator] " + result.ErrorMessage);
            }
            else
            {
                Debug.Log("[L2EffectGenerator] prominence regenerate done (request runner)");
            }
        }
        finally
        {
            try
            {
                if (File.Exists(busy))
                {
                    File.Delete(busy);
                }
            }
            catch
            {
                // ignore
            }
        }
    }
}
#endif
