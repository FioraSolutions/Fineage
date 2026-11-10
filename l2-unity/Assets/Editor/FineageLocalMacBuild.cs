using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// StandaloneOSX player for local Fineage-Backend (login 9106 / zone 9777).
/// Launch with <c>FINEAGE_NET_CLIENT=1</c> (see <c>launch-local.sh</c> next to the app).
/// </summary>
public static class FineageLocalMacBuild
{
    const string SentinelName = ".fineage-build-local-mac";

    [MenuItem("Fineage/Build Local Mac (9106)")]
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
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "Fineage local macOS build failed: " + report.summary.result
                    + " errors=" + report.summary.totalErrors);
            }

            File.WriteAllText(Path.Combine(dir, "LAUNCH.txt"), LaunchText());
            string launchSh = Path.Combine(dir, "launch-local.sh");
            File.WriteAllText(launchSh, LaunchScript());
            // +x for double-click / shell
            try
            {
                var chmod = System.Diagnostics.Process.Start("chmod", "+x \"" + launchSh + "\"");
                chmod?.WaitForExit(5000);
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[FineageLocal] chmod launch-local.sh failed: " + ex.Message);
            }

            Debug.Log("[FineageLocal] built " + app);
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
        return "Fineage local macOS client\n"
            + "\n"
            + "Requires Fineage-Backend run-local-net (login 9106, zone 9777).\n"
            + "\n"
            + "Launch:\n"
            + "  ./launch-local.sh\n"
            + "\n"
            + "Accounts: localplay / localplay2 (same password as localplay).\n"
            + "Chars: KuK / KuK2\n";
    }

    static string LaunchScript()
    {
        // Run the Mach-O directly so FINEAGE_NET_CLIENT reaches GameManager (open(1) drops env).
        return "#!/bin/bash\n"
            + "set -euo pipefail\n"
            + "cd \"$(dirname \"$0\")\"\n"
            + "export FINEAGE_NET_CLIENT=1\n"
            + "exec \"./Fineage.app/Contents/MacOS/Fineage\" \"$@\"\n";
    }
}

