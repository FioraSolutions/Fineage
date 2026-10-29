using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// StandaloneOSX player for the public RP. No FINEAGE_LOCAL_BACKEND.
/// Hosts stay the committed playtest endpoints (login/game/api on evpoint.by).
/// </summary>
public static class FineagePublicMacBuild
{
    public static void Build()
    {
        string dir = Environment.GetEnvironmentVariable("FINEAGE_PUBLIC_OUT");
        if (string.IsNullOrWhiteSpace(dir))
            dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Fineage-macOS-dev-create");

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
                extraScriptingDefines = new[] { "FINEAGE_PUBLIC_PLAYTEST" }
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "Fineage public macOS build failed: " + report.summary.result
                    + " errors=" + report.summary.totalErrors);
            }

            File.WriteAllText(Path.Combine(dir, "LAUNCH.txt"), LaunchText());
            Debug.Log("[FineagePublic] built " + app);
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
        return "Fineage public RP macOS client (dev)\n"
            + "\n"
            + "Login TCP  login.fineage.evpoint.by:2106\n"
            + "Game TCP   game.fineage.evpoint.by:7777\n"
            + "API        api-fineage.evpoint.by\n"
            + "\n"
            + "This player does not dial localhost.\n"
            + "Double-click Fineage.app.\n";
    }
}
