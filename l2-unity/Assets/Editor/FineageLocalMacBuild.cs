using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// Developer StandaloneOSX player carrying both environments. It is compiled with
/// <c>FINEAGE_LOCAL_CAPABLE</c>, so <c>FINEAGE_BACKEND</c> (or the legacy
/// <c>FINEAGE_NET_CLIENT=1</c>) can point it at loopback.
///
/// <para>
/// The app itself dials <b>remote</b>; the folder ships <c>launch.sh</c> (remote) and
/// <c>launch-local.sh</c> (Fineage-Backend on 127.0.0.1:9106/9777). Release-clients builds
/// do not carry the define, so they can never leave the remote server.
/// </para>
/// </summary>
public static class FineageLocalMacBuild
{
    const string SentinelName = ".fineage-build-local-mac";

    [MenuItem("Fineage/Build Dev Mac (remote + local)")]
    public static void BuildMenu() => Build();

    static string _outOverride;

    [InitializeOnLoadMethod]
    static void MaybeBuildFromSentinel()
    {
        EditorApplication.delayCall += () =>
        {
            string root = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            string sentinel = Path.Combine(root, SentinelName);
            if (!File.Exists(sentinel))
                return;

            string outDir = null;
            try
            {
                outDir = File.ReadAllText(sentinel).Trim();
                File.Delete(sentinel);
            }
            catch { return; }

            string done = sentinel + ".done";
            string fail = sentinel + ".fail";
            try
            {
                if (File.Exists(done)) File.Delete(done);
                if (File.Exists(fail)) File.Delete(fail);
                if (!string.IsNullOrWhiteSpace(outDir))
                    _outOverride = outDir;
                Build();
                File.WriteAllText(done, DateTime.UtcNow.ToString("o") + "\n" + (_outOverride ?? "") + "\n");
            }
            catch (Exception ex)
            {
                File.WriteAllText(fail, ex.ToString());
                Debug.LogException(ex);
            }
            finally
            {
                _outOverride = null;
            }
        };
    }

    public static void Build()
    {
        string dir = _outOverride;
        if (string.IsNullOrWhiteSpace(dir))
            dir = Environment.GetEnvironmentVariable("FINEAGE_LOCAL_OUT");
        if (string.IsNullOrWhiteSpace(dir))
            dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Fineage-macOS-local");

        Directory.CreateDirectory(dir);
        string app = Path.Combine(dir, "Fineage.app");
        if (Directory.Exists(app))
            Directory.Delete(app, true);

        string previousName = PlayerSettings.productName;
        int previousWidth = PlayerSettings.defaultScreenWidth;
        int previousHeight = PlayerSettings.defaultScreenHeight;
        FullScreenMode previousFullScreen = PlayerSettings.fullScreenMode;
        PlayerSettings.productName = "Fineage";
        PlayerSettings.defaultScreenWidth = 1920;
        PlayerSettings.defaultScreenHeight = 1080;
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        try
        {
            string[] scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();
            if (scenes.Length == 0)
                throw new InvalidOperationException("No enabled scenes in EditorBuildSettings.");

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = app,
                target = BuildTarget.StandaloneOSX,
                targetGroup = BuildTargetGroup.Standalone,
                options = BuildOptions.None,
                // The one binary allowed to choose loopback at launch. A release build
                // without this define resolves Remote no matter what the environment says.
                extraScriptingDefines = new[] { "FINEAGE_LOCAL_CAPABLE" }
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "Fineage local macOS build failed: " + report.summary.result
                    + " errors=" + report.summary.totalErrors);
            }

            File.WriteAllText(Path.Combine(dir, "LAUNCH.txt"), LaunchText());
            WriteExecutable(Path.Combine(dir, "launch.sh"), LaunchScript());
            WriteExecutable(Path.Combine(dir, "launch-local.sh"), LaunchLocalScript());

            Debug.Log("[FineageLocal] built " + app + " (remote by default; launch-local.sh → loopback)");
        }
        finally
        {
            PlayerSettings.productName = previousName;
            PlayerSettings.defaultScreenWidth = previousWidth;
            PlayerSettings.defaultScreenHeight = previousHeight;
            PlayerSettings.fullScreenMode = previousFullScreen;
        }
    }

    static string LaunchText()
    {
        return "Fineage dev macOS client — pick the backend at launch\n"
            + "\n"
            + "  ./launch.sh         remote playtest (default)\n"
            + "                      login.fineage.evpoint.by:2106 / game.fineage.evpoint.by:7777\n"
            + "  ./launch-local.sh   local Fineage-Backend\n"
            + "                      127.0.0.1:9106 / zone 127.0.0.1:9777\n"
            + "\n"
            + "The local one needs Fineage-Backend scripts/run-local-net.sh.\n"
            + "Accounts: localplay / localplay2 (same password as localplay).\n"
            + "Chars: KuK / KuK2\n"
            + "\n"
            + "Running Fineage.app directly is remote. The scripts pass FINEAGE_BACKEND\n"
            + "to the Mach-O because open(1) drops the environment.\n";
    }

    const string LaunchHeader =
        "#!/bin/bash\n"
        + "set -euo pipefail\n"
        + "cd \"$(dirname \"$0\")\"\n"
        + "export FINEAGE_BACKEND=%BACKEND%\n"
        + "exec \"./Fineage.app/Contents/MacOS/Fineage\" \"$@\"\n";

    static string LaunchScript() => LaunchHeader.Replace("%BACKEND%", "remote");

    static string LaunchLocalScript() => LaunchHeader.Replace("%BACKEND%", "local");

    static void WriteExecutable(string path, string contents)
    {
        File.WriteAllText(path, contents);
        try
        {
            var chmod = System.Diagnostics.Process.Start("chmod", "+x \"" + path + "\"");
            chmod?.WaitForExit(5000);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[FineageLocal] chmod " + Path.GetFileName(path) + " failed: " + ex.Message);
        }
    }
}

