using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Opens Menu and enters play mode when /tmp/fineage-interlude-splash.outpath
/// names a PNG. InterludeSplashPlayer writes that file once the splash is up.
/// </summary>
[InitializeOnLoad]
public static class InterludeSplashShot
{
    const string MenuScenePath = "Assets/Resources/Scenes/Menu.unity";

    static bool _playRequested;
    static bool _exitQueued;
    static double _armedAt;
    static double _playDeadline;
    static string _shotPath;

    static InterludeSplashShot()
    {
        EditorApplication.update += Poll;
    }

    [MenuItem("Fineage/Play Interlude Splash")]
    public static void PlaySplash()
    {
        OpenMenuThenPlay();
    }

    static void Poll()
    {
        if (_exitQueued)
            return;

        if (string.IsNullOrEmpty(_shotPath))
        {
            if (!File.Exists(InterludeSplashPlayer.ShotPathFile))
                return;
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
                return;
            if (EditorUtility.scriptCompilationFailed)
            {
                Debug.LogError("[InterludeSplash] scripts failed to compile");
                Exit(1);
                return;
            }

            try
            {
                _shotPath = File.ReadAllText(InterludeSplashPlayer.ShotPathFile).Trim();
            }
            catch (IOException ex)
            {
                Debug.LogError("[InterludeSplash] could not read shot path: " + ex.Message);
                return;
            }

            if (string.IsNullOrEmpty(_shotPath))
                return;

            _armedAt = EditorApplication.timeSinceStartup;
            Debug.Log("[InterludeSplash] armed proof shot " + _shotPath);
        }

        if (EditorApplication.isPlaying)
        {
            if (File.Exists(_shotPath))
            {
                long size = new FileInfo(_shotPath).Length;
                if (size > 10000)
                {
                    Debug.Log("[InterludeSplash] proof shot ready bytes=" + size);
                    try
                    {
                        if (File.Exists(InterludeSplashPlayer.ShotPathFile))
                            File.Delete(InterludeSplashPlayer.ShotPathFile);
                    }
                    catch (IOException)
                    {
                        /* leave it */
                    }

                    Exit(0);
                    return;
                }
            }

            if (_playDeadline <= 0)
                _playDeadline = EditorApplication.timeSinceStartup + 150.0;
            if (EditorApplication.timeSinceStartup > _playDeadline)
            {
                Debug.LogError("[InterludeSplash] timed out waiting for " + _shotPath);
                Exit(1);
            }

            return;
        }

        if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling || EditorApplication.isUpdating)
            return;

        if (!_playRequested)
        {
            _playRequested = true;
            OpenMenuThenPlay();
            return;
        }

        if (EditorApplication.timeSinceStartup > _armedAt + 180.0)
        {
            Debug.LogError("[InterludeSplash] play mode did not start");
            Exit(1);
        }
    }

    static void OpenMenuThenPlay()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
        {
            EditorApplication.delayCall += OpenMenuThenPlay;
            return;
        }

        var scene = EditorSceneManager.GetActiveScene();
        if (scene.path != MenuScenePath)
            EditorSceneManager.OpenScene(MenuScenePath, OpenSceneMode.Single);

        System.Type gameView = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
        if (gameView != null)
        {
            EditorWindow window = EditorWindow.GetWindow(gameView, false, null, false);
            if (window != null)
                window.minSize = new Vector2(960, 540);
        }

        EditorApplication.isPlaying = true;
    }

    static void Exit(int code)
    {
        if (_exitQueued)
            return;
        _exitQueued = true;
        EditorApplication.Exit(code);
    }
}
