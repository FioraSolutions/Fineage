#if UNITY_EDITOR
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Regenerates locomotion stride sidecars from the committed .anim clips.
/// The clips are in-place, so the bake reads root and foot curves directly
/// (tools/locomotion-stride/bake_strides.py). No Mac-only rebake is required
/// unless the .anim files themselves change.
///
///   Unity -batchmode -quit -projectPath l2-unity -executeMethod L2LocomotionStrideBake.RunCli
///   python3 tools/locomotion-stride/bake_strides.py
/// </summary>
public static class L2LocomotionStrideBake
{
    [MenuItem("Shnok/Characters/Bake locomotion strides")]
    public static void Menu()
    {
        int code = Run();
        UnityEngine.Debug.Log("[L2LocomotionStrideBake] exit " + code);
    }

    public static void RunCli()
    {
        int code = Run();
        EditorApplication.Exit(code == 0 ? 0 : 1);
    }

    public static int Run()
    {
        string repo = Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".."));
        string script = Path.Combine(repo, "tools", "locomotion-stride", "bake_strides.py");
        if (!File.Exists(script))
        {
            UnityEngine.Debug.LogError("[L2LocomotionStrideBake] missing " + script);
            return 1;
        }

        var psi = new ProcessStartInfo
        {
            FileName = "python3",
            Arguments = "\"" + script + "\"",
            WorkingDirectory = repo,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        using (Process process = Process.Start(psi))
        {
            string stdout = process.StandardOutput.ReadToEnd();
            string stderr = process.StandardError.ReadToEnd();
            process.WaitForExit();
            UnityEngine.Debug.Log("[L2LocomotionStrideBake]\n" + stdout + stderr);
            AssetDatabase.Refresh();
            return process.ExitCode;
        }
    }
}
#endif
