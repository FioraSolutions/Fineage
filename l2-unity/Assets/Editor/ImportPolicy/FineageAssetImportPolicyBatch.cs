#if UNITY_EDITOR
using System.Diagnostics;
using UnityEditor;

/// <summary>
/// One-shot batch entry point; CI uses <c>scripts/check-asset-import-policy.py --apply</c> instead.
/// </summary>
public static class FineageAssetImportPolicyBatch
{
    const string ScriptRelative = "scripts/check-asset-import-policy.py";

    [MenuItem("Fineage/Import Policy/Re-apply rules to all .meta files (python)")]
    public static void MenuApplyViaPython()
    {
        string repoRoot = System.IO.Path.GetFullPath(System.IO.Path.Combine(UnityEngine.Application.dataPath, "../.."));
        string unityRoot = System.IO.Path.GetFullPath(System.IO.Path.Combine(UnityEngine.Application.dataPath, ".."));
        string script = System.IO.Path.Combine(repoRoot, ScriptRelative);
        var start = new ProcessStartInfo
        {
            FileName = "python3",
            Arguments = "\"" + script + "\" --apply --report \"" + unityRoot + "/ImportPolicy/last-apply-report.json\"",
            WorkingDirectory = repoRoot,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };
        using var proc = Process.Start(start);
        string stdout = proc.StandardOutput.ReadToEnd();
        string stderr = proc.StandardError.ReadToEnd();
        proc.WaitForExit();
        UnityEngine.Debug.Log("[ImportPolicy] exit=" + proc.ExitCode + "\n" + stdout + stderr);
        if (proc.ExitCode == 0)
            AssetDatabase.Refresh();
    }

    public static void RunCli()
    {
        MenuApplyViaPython();
        EditorApplication.Exit(0);
    }
}
#endif