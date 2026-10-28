using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// StandaloneOSX player that defaults to the local C# backend via FINEAGE_LOCAL_BACKEND.
/// The open editor consumes /tmp/fineage-local-build.request once after this script compiles.
/// </summary>
[InitializeOnLoad]
public static class FineageLocalMacBuild
{
    const string RequestPath = "/tmp/fineage-local-build.request";

    static FineageLocalMacBuild()
    {
        EditorApplication.delayCall += TryConsumeRequest;
    }

    static void TryConsumeRequest()
    {
        if (!File.Exists(RequestPath))
            return;

        try
        {
            File.Delete(RequestPath);
        }
        catch (Exception ex)
        {
            Debug.LogError("[FineageLocal] could not consume build request: " + ex.Message);
            return;
        }

        Build();
    }

    public static void Build()
    {
        string dir = Environment.GetEnvironmentVariable("FINEAGE_LOCAL_OUT");
        if (string.IsNullOrWhiteSpace(dir))
            dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Fineage-macOS-local");

        Directory.CreateDirectory(dir);
        string app = Path.Combine(dir, "Fineage.app");
        if (Directory.Exists(app))
            Directory.Delete(app, true);

        string previousName = PlayerSettings.productName;
        PlayerSettings.productName = "Fineage";
        try
        {
            string[] scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();
            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = app,
                target = BuildTarget.StandaloneOSX,
                targetGroup = BuildTargetGroup.Standalone,
                options = BuildOptions.None,
                extraScriptingDefines = new[] { "FINEAGE_LOCAL_BACKEND" }
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "Fineage local macOS build failed: " + report.summary.result
                    + " errors=" + report.summary.totalErrors);
            }

            File.WriteAllText(Path.Combine(dir, "LAUNCH.txt"), LaunchText(app));
            Debug.Log("[FineageLocal] built " + app);
        }
        finally
        {
            PlayerSettings.productName = previousName;
        }
    }

    static string LaunchText(string app)
    {
        return "Fineage local macOS client\n"
            + "This build logs into the C# Fineage-Backend on this Mac.\n"
            + "\n"
            + "Login TCP  127.0.0.1:9106\n"
            + "Login HTTP http://127.0.0.1:5080\n"
            + "Zone TCP   127.0.0.1:9777\n"
            + "\n"
            + "Account localtest\n"
            + "Password localtest1\n"
            + "Character Hero (used when it is the only character, or pass -fineageChar Hero)\n"
            + "\n"
            + "Launch:\n"
            + "  open \"" + app + "\"\n"
            + "\n"
            + "Auto-login:\n"
            + "  \"" + app + "/Contents/MacOS/Fineage\" -fineageUser localtest -fineagePass localtest1 -fineageChar Hero\n"
            + "\n"
            + "This player defaults to 127.0.0.1. Pass -fineagePublic to use the Interlude client path instead.\n"
            + "Other builds stay on their current endpoints unless started with -fineageLocal.\n";
    }
}
