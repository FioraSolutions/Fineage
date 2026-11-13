using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// Android APK for the public RP.
/// Hosts stay the committed playtest endpoints (login/game/api on evpoint.by).
/// Drop <c>.fineage-build-public-android</c> in the Unity project root to start a build
/// from an already-open editor.
/// </summary>
public static class FineagePublicAndroidBuild
{
    const string SentinelName = ".fineage-build-public-android";

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
            catch
            {
                return;
            }

            string done = sentinel + ".done";
            string fail = sentinel + ".fail";
            try
            {
                if (File.Exists(done)) File.Delete(done);
                if (File.Exists(fail)) File.Delete(fail);
                Build(outDir);
                File.WriteAllText(done, DateTime.UtcNow.ToString("o") + "\n");
            }
            catch (Exception ex)
            {
                File.WriteAllText(fail, ex.ToString());
                Debug.LogException(ex);
            }
        };
    }

    public static void Build()
    {
        Build(null);
    }

    static void Build(string dir)
    {
        string player = Path.Combine(EditorApplication.applicationContentsPath, "PlaybackEngines", "AndroidPlayer");
        if (!Directory.Exists(player))
            throw new InvalidOperationException("Android Build Support is not installed at " + player);

        if (string.IsNullOrWhiteSpace(dir))
            dir = Environment.GetEnvironmentVariable("FINEAGE_PUBLIC_ANDROID_OUT");
        if (string.IsNullOrWhiteSpace(dir))
            dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Fineage-Android-public");

        Directory.CreateDirectory(dir);
        string apk = Path.Combine(dir, "Fineage.apk");
        if (File.Exists(apk))
            File.Delete(apk);

        string previousName = PlayerSettings.productName;
        string previousId = PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Android);
        var previousOrientation = PlayerSettings.defaultInterfaceOrientation;
        var previousBackend = PlayerSettings.GetScriptingBackend(NamedBuildTarget.Android);
        BuildTarget previousTarget = EditorUserBuildSettings.activeBuildTarget;
        PlayerSettings.productName = "Fineage";
        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "by.evpoint.fineage");
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        // ARM64 Android players in this editor are IL2CPP. Mono has no supported CPU here.
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
        ForceArm64();
        string previousBanks = FMODUnity.Settings.Instance != null
            ? FMODUnity.Settings.Instance.SourceBankPath
            : null;
        if (FMODUnity.Settings.Instance != null)
            FMODUnity.Settings.Instance.SourceBankPath = "";
        try
        {
            string[] scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();
            if (scenes.Length == 0)
                throw new InvalidOperationException("No enabled scenes in EditorBuildSettings.");

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = apk,
                target = BuildTarget.Android,
                targetGroup = BuildTargetGroup.Android,
                options = BuildOptions.CompressWithLz4,
                extraScriptingDefines = new[] { "FINEAGE_PUBLIC_PLAYTEST" }
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "Fineage public Android build failed: " + report.summary.result
                    + " errors=" + report.summary.totalErrors);
            }

            File.WriteAllText(Path.Combine(dir, "LAUNCH.txt"), LaunchText());
            Debug.Log("[FineagePublic] built " + apk);
        }
        finally
        {
            if (FMODUnity.Settings.Instance != null && previousBanks != null)
                FMODUnity.Settings.Instance.SourceBankPath = previousBanks;
            PlayerSettings.productName = previousName;
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, previousId);
            PlayerSettings.defaultInterfaceOrientation = previousOrientation;
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, previousBackend);
            if (previousTarget == BuildTarget.StandaloneOSX
                && string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FINEAGE_ANDROID_NO_SWITCH")))
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX);
        }
    }

    static void ForceArm64()
    {
        Type android = null;
        foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            android = asm.GetType("UnityEditor.PlayerSettings+Android");
            if (android != null)
                break;
        }

        if (android == null)
            throw new InvalidOperationException("PlayerSettings.Android is unavailable. Android Build Support is not loaded.");

        PropertyInfo prop = android.GetProperty("targetArchitectures", BindingFlags.Public | BindingFlags.Static);
        if (prop == null)
            throw new InvalidOperationException("PlayerSettings.Android.targetArchitectures is unavailable.");

        prop.SetValue(null, Enum.ToObject(prop.PropertyType, 2));
    }

    static string LaunchText()
    {
        return "Fineage public RP Android client (dev)\n"
            + "\n"
            + "Login TCP  login.fineage.evpoint.by:2106\n"
            + "Game TCP   game.fineage.evpoint.by:7777\n"
            + "API        api-fineage.evpoint.by\n"
            + "\n"
            + "This player does not dial localhost.\n"
            + "Install Fineage.apk (arm64). Package by.evpoint.fineage.\n"
            + "FMOD Android native libs are not in the repo; sound may be silent.\n";
    }
}
