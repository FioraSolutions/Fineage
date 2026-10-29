using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Opens Menu and enters play mode when /tmp/fineage-interlude-login.outpath
/// names a PNG. InterludeLoginProof writes that file once the credentials window is up.
/// </summary>
[InitializeOnLoad]
public static class InterludeLoginShot
{
    const string MenuScenePath = "Assets/Resources/Scenes/Menu.unity";

    static bool _playRequested;
    static bool _exitQueued;
    static double _armedAt;
    static double _playDeadline;
    static string _shotPath;

    static InterludeLoginShot()
    {
        EditorApplication.update += Poll;
    }

    static void Poll()
    {
        if (_exitQueued)
            return;

        if (string.IsNullOrEmpty(_shotPath))
        {
            if (!File.Exists(InterludeLoginProof.ShotPathFile))
                return;
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
                return;
            if (EditorUtility.scriptCompilationFailed)
            {
                Debug.LogError("[InterludeLogin] scripts failed to compile");
                Exit(1);
                return;
            }

            try
            {
                _shotPath = File.ReadAllText(InterludeLoginProof.ShotPathFile).Trim();
            }
            catch (IOException ex)
            {
                Debug.LogError("[InterludeLogin] could not read shot path: " + ex.Message);
                return;
            }

            if (string.IsNullOrEmpty(_shotPath))
                return;

            _armedAt = EditorApplication.timeSinceStartup;
            Debug.Log("[InterludeLogin] armed proof shot " + _shotPath);
        }

        if (EditorApplication.isPlaying)
        {
            if (File.Exists(_shotPath))
            {
                long size = new FileInfo(_shotPath).Length;
                if (size > 10000)
                {
                    Debug.Log("[InterludeLogin] proof shot ready bytes=" + size);
                    try
                    {
                        if (File.Exists(InterludeLoginProof.ShotPathFile))
                            File.Delete(InterludeLoginProof.ShotPathFile);
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
                _playDeadline = EditorApplication.timeSinceStartup + 90.0;
            if (EditorApplication.timeSinceStartup > _playDeadline)
            {
                Debug.LogError("[InterludeLogin] timed out waiting for " + _shotPath);
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
            Debug.LogError("[InterludeLogin] play mode did not start");
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
                window.minSize = new Vector2(1280, 720);
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
