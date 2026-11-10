#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Polls Temp/*.request while the Editor already holds the project lock
/// (batchmode cannot attach).
/// </summary>
[InitializeOnLoad]
public static class L2EffectGeneratorRequestRunner
{
    const string ProminenceRequest = "Temp/l2-effect-generate-prominence.request";
    const string ProminenceDone = "Temp/l2-effect-generate-prominence.done";
    const string ProminenceBusy = "Temp/l2-effect-generate-prominence.busy";

    const string MageBatchRequest = "Temp/l2-effect-generate-human-mage.request";
    const string MageBatchDone = "Temp/l2-effect-generate-human-mage.done";
    const string MageBatchBusy = "Temp/l2-effect-generate-human-mage.busy";

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
        TryRun(
            projectRoot,
            MageBatchRequest,
            MageBatchBusy,
            MageBatchDone,
            RunHumanMageBatch);
        TryRun(
            projectRoot,
            ProminenceRequest,
            ProminenceBusy,
            ProminenceDone,
            RunProminence);
    }

    delegate void RequestAction(out bool success, out string body);

    static void TryRun(
        string projectRoot,
        string requestRelative,
        string busyRelative,
        string doneRelative,
        RequestAction action)
    {
        string request = Path.Combine(projectRoot, requestRelative);
        string busy = Path.Combine(projectRoot, busyRelative);
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
            action(out bool success, out string body);
            File.WriteAllText(
                Path.Combine(projectRoot, doneRelative),
                (success ? "ok\n" : "fail\n") + body);
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

    static void RunHumanMageBatch(out bool success, out string body)
    {
        AssetDatabase.Refresh();
        success = L2EffectGeneratorWindow.TryRunHumanMageBatch(out string summary);
        body = summary ?? string.Empty;
        if (success)
        {
            Debug.Log("[L2EffectGenerator] human mage batch done (request runner)");
        }
        else
        {
            Debug.LogError("[L2EffectGenerator] human mage batch failed: " + body);
        }
    }

    static void RunProminence(out bool success, out string body)
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

        success = result.Success;
        body = result.Success
            ? string.Join("\n", result.LogLines)
            : result.ErrorMessage + "\n" + string.Join("\n", result.LogLines);

        if (!result.Success)
        {
            Debug.LogError("[L2EffectGenerator] " + result.ErrorMessage);
        }
        else
        {
            Debug.Log("[L2EffectGenerator] prominence regenerate done (request runner)");
        }
    }
}
#endif
