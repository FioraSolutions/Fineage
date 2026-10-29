using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Public RP play mode from the Unity Editor (Menu login scene → login.fineage.evpoint.by:2106).
/// An open editor consumes <c>/tmp/fineage-rp-editor-play.request</c> once after this script compiles.
/// </summary>
[InitializeOnLoad]
public static class FineageRpEditorPlay
{
    const string RequestPath = "/tmp/fineage-rp-editor-play.request";
    const string MenuScenePath = "Assets/Resources/Scenes/Menu.unity";

    static bool _updateHooked;
    static bool _restartPlayAfterStop;

    static FineageRpEditorPlay()
    {
        EditorApplication.delayCall += TryConsumeRequest;
        if (!_updateHooked)
        {
            _updateHooked = true;
            EditorApplication.update += PollRequest;
        }
    }

    static double _nextPollTime;

    static void PollRequest()
    {
        if (EditorApplication.timeSinceStartup < _nextPollTime)
            return;
        _nextPollTime = EditorApplication.timeSinceStartup + 2.0;
        if (File.Exists(RequestPath))
            TryConsumeRequest();
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
            Debug.LogError("[FineageRP] could not consume play request: " + ex.Message);
            return;
        }

        StartPlayRp();
    }

    [MenuItem("Fineage/Play RP")]
    public static void PlayFromMenu()
    {
        StartPlayRp();
    }

    static void StartPlayRp()
    {
        EnsurePublicRpMode();

        if (EditorApplication.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode)
        {
            if (_restartPlayAfterStop)
                return;

            _restartPlayAfterStop = true;
            EditorApplication.playModeStateChanged += OnPlayModeChangedForRestart;
            EditorApplication.isPlaying = false;
            return;
        }

        OpenMenuAndEnterPlayMode();
    }

    static void OnPlayModeChangedForRestart(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.EnteredEditMode || !_restartPlayAfterStop)
            return;

        EditorApplication.playModeStateChanged -= OnPlayModeChangedForRestart;
        _restartPlayAfterStop = false;
        EditorApplication.delayCall += OpenMenuAndEnterPlayMode;
    }

    static void OpenMenuAndEnterPlayMode()
    {
        if (!File.Exists(MenuScenePath))
        {
            Debug.LogError("[FineageRP] Menu scene missing at " + MenuScenePath);
            return;
        }

        EditorSceneManager.OpenScene(MenuScenePath, OpenSceneMode.Single);
        LogPublicLoginEndpoint();
        EditorApplication.isPlaying = true;
    }

    /// <summary>Public dev RP — not the local C# backend path. Clears a stale character hint.</summary>
    static void EnsurePublicRpMode()
    {
        Environment.SetEnvironmentVariable("FINEAGE_LOCAL_BACKEND", "0");
        Environment.SetEnvironmentVariable("FINEAGE_LOCAL_CHAR", null);
        if (PlayerPrefs.GetInt(FineageLocalBackend.PrefKey, 0) != 0)
            PlayerPrefs.DeleteKey(FineageLocalBackend.PrefKey);
        FineageLocalBackend.ClearCharacterHint();
        PlayerPrefs.Save();
        Debug.Log("[FineageRP] Editor Play → public RP ("
            + PlaytestEndpoint.DefaultLoginHost + ":" + PlaytestEndpoint.DefaultLoginPort
            + "). Character hint cleared.");
    }

    static void LogPublicLoginEndpoint()
    {
        Debug.Log(
            "[FineageRP] login="
            + PlaytestEndpoint.DefaultLoginHost
            + ":"
            + PlaytestEndpoint.DefaultLoginPort);
    }
}
