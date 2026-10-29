using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Editor Play proof for the forest login vista. Armed by
/// /tmp/fineage-login-forest.outpath (not the splash shot path).
/// </summary>
[InitializeOnLoad]
public static class LoginForestShot
{
    const string MenuScenePath = "Assets/Resources/Scenes/Menu.unity";

    static bool _playRequested;
    static bool _exitQueued;
    static double _armedAt;
    static double _playDeadline;
    static string _shotPath;

    static LoginForestShot()
    {
        EditorApplication.update += Poll;
    }

    static void Poll()
    {
        if (_exitQueued)
            return;

        if (string.IsNullOrEmpty(_shotPath))
        {
            if (!File.Exists(LoginForestPresenter.ShotPathFile))
                return;
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
                return;
            if (EditorUtility.scriptCompilationFailed)
            {
                Debug.LogError("[LoginForest] scripts failed to compile");
                Exit(1);
                return;
            }

            try
            {
                _shotPath = File.ReadAllText(LoginForestPresenter.ShotPathFile).Trim();
            }
            catch (IOException ex)
            {
                Debug.LogError("[LoginForest] could not read shot path: " + ex.Message);
                return;
            }

            if (string.IsNullOrEmpty(_shotPath))
                return;

            _armedAt = EditorApplication.timeSinceStartup;
            ReimportLoginMeshes();
            Debug.Log("[LoginForest] armed proof shot " + _shotPath);
        }

        if (EditorApplication.isPlaying)
        {
            if (File.Exists(_shotPath))
            {
                long size = new FileInfo(_shotPath).Length;
                if (size > 10000)
                {
                    Debug.Log("[LoginForest] proof shot ready bytes=" + size);
                    try
                    {
                        if (File.Exists(LoginForestPresenter.ShotPathFile))
                            File.Delete(LoginForestPresenter.ShotPathFile);
                    }
                    catch (IOException)
                    {
                    }

                    Exit(0);
                    return;
                }
            }

            if (_playDeadline <= 0)
                _playDeadline = EditorApplication.timeSinceStartup + 150.0;
            if (EditorApplication.timeSinceStartup > _playDeadline)
            {
                Debug.LogError("[LoginForest] timed out waiting for " + _shotPath);
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

        if (EditorApplication.timeSinceStartup > _armedAt + 240.0)
        {
            Debug.LogError("[LoginForest] play mode did not start");
            Exit(1);
        }
    }

    static void ReimportLoginMeshes()
    {
        string[] files =
        {
            "Assets/Resources/Data/StaticMeshes/L2_Lobby/lobby_back.fbx",
            "Assets/Resources/Data/StaticMeshes/L2_Lobby/L2_Lobby_sky_elf.fbx",
        };
        for (int i = 0; i < files.Length; i++)
        {
            if (File.Exists(files[i]))
                AssetDatabase.ImportAsset(files[i], ImportAssetOptions.ForceUpdate);
        }

        ImportFolder("Assets/Resources/Data/StaticMeshes/Primitive_FieldOBJ_S");
        string[] deco =
        {
            "mushroom1", "mushroom3", "Aden_grass_M01", "Aden_grass_M03", "dion_grass008",
            "Sporemushroom1", "Sporemushroom2", "Sporemushroom3", "Oren_elf_grass0044",
            "oren_grass003", "dion_grass001", "poison_flower", "Aden_gardengrass002",
            "dion_grass007", "aden_Giantgrass01", "Aden_gardengrass006", "dion_grass010",
            "aden_Giantgrass02", "Aden_gardengrass005", "aden_Giantgrass03", "dion_grass011",
            "Aden_Stone006",
        };
        for (int i = 0; i < deco.Length; i++)
        {
            string path = "Assets/Resources/Data/StaticMeshes/field_deco_S/" + deco[i] + ".fbx";
            if (File.Exists(path))
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }

        AssetDatabase.Refresh();
    }

    static void ImportFolder(string folder)
    {
        if (!Directory.Exists(folder))
            return;
        string[] fbx = Directory.GetFiles(folder, "*.fbx");
        for (int i = 0; i < fbx.Length; i++)
            AssetDatabase.ImportAsset(fbx[i].Replace('\\', '/'), ImportAssetOptions.ForceUpdate);
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
