using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

/// <summary>
/// Editor Play proof for the forest login vista. Armed by
/// /tmp/fineage-login-forest.outpath (not the splash shot path).
/// Captures from the editor poll once the LoginWindow is shown
/// (the vista itself is the baked lobby scene — no presenter).
/// </summary>
[InitializeOnLoad]
public static class LoginForestShot
{
    public const string ShotPathFile = "/tmp/fineage-login-forest.outpath";
    const string MenuScenePath = "Assets/Resources/Scenes/Menu.unity";

    static bool _playRequested;
    static bool _exitQueued;
    static double _armedAt;
    static double _playDeadline;
    static string _shotPath;
    static int _captureReadyFrames;
    static bool _captureRequested;
    static DateTime _captureRequestedUtc;

    static LoginForestShot()
    {
        EditorApplication.update += Poll;
    }

    /// <summary>CLI: Unity -batchmode -projectPath l2-unity -executeMethod LoginForestShot.RunCli</summary>
    public static void RunCli()
    {
        string dest = "/Users/nikitarakitin/Desktop/fineage-login-forest-proof.png";
        string[] args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (string.Equals(args[i], "-shotOut", System.StringComparison.OrdinalIgnoreCase))
                dest = args[i + 1];
        }

        for (int i = 0; i < args.Length; i++)
        {
            if (string.Equals(args[i], "-quit", System.StringComparison.OrdinalIgnoreCase))
            {
                Debug.LogError(
                    "[LoginForest] -quit exits when RunCli returns, before Poll() enters play mode. "
                    + "Run without -quit; the shot calls EditorApplication.Exit.");
            }
        }

        Directory.CreateDirectory(Path.GetDirectoryName(dest) ?? "/tmp");
        File.WriteAllText(ShotPathFile, dest);
        Debug.Log("[LoginForest] RunCli armed " + dest);
        // Poll() on update drives play + capture + Exit. Do not pass -quit:
        // Unity leaves batchmode when executeMethod returns, before play mode.
    }

    static void Poll()
    {
        if (_exitQueued)
            return;

        if (string.IsNullOrEmpty(_shotPath))
        {
            if (!File.Exists(ShotPathFile))
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
                _shotPath = File.ReadAllText(ShotPathFile).Trim();
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
            if (!_captureRequested)
            {
                if (LoginWindowShown())
                    _captureReadyFrames++;
                else
                    _captureReadyFrames = 0;
                if (_captureReadyFrames >= 3)
                {
                    LogCaptureSurface();
                    WriteSceneOnlyProof(SceneProofPath(_shotPath));
                    try
                    {
                        if (File.Exists(_shotPath))
                            File.Delete(_shotPath);
                    }
                    catch (IOException ex)
                    {
                        Debug.LogError("[LoginForest] could not clear previous proof: " + ex.Message);
                    }

                    _captureRequested = true;
                    _captureRequestedUtc = DateTime.UtcNow;
                    ScreenCapture.CaptureScreenshot(_shotPath);
                }
            }

            if (_captureRequested && File.Exists(_shotPath))
            {
                var info = new FileInfo(_shotPath);
                info.Refresh();
                bool writtenAfterRequest = info.LastWriteTimeUtc >= _captureRequestedUtc.AddSeconds(-2);
                if (info.Length > 10000 && writtenAfterRequest)
                {
                    Debug.Log("[LoginForest] proof shot ready bytes=" + info.Length +
                        " scene=" + SceneProofPath(_shotPath));
                    try
                    {
                        if (File.Exists(ShotPathFile))
                            File.Delete(ShotPathFile);
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

    static string SceneProofPath(string shotPath)
    {
        string dir = Path.GetDirectoryName(shotPath) ?? "";
        string name = Path.GetFileNameWithoutExtension(shotPath);
        return Path.Combine(dir, name + "-3d.png");
    }

    static void LogCaptureSurface()
    {
        var scenes = new StringBuilder();
        bool lobbyLoaded = false;
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scenes.Length > 0)
                scenes.Append(", ");
            scenes.Append(scene.name);
            scenes.Append(scene.isLoaded ? "=loaded" : "=unloaded");
            if (scene.isLoaded && scene.name == LobbyMapInterludeContract.AdditiveLobbySceneName)
                lobbyLoaded = true;
        }

        Debug.Log("[LoginForest] scenes " + scenes + " lobbyLoaded=" + lobbyLoaded);

        Camera[] cameras = UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < cameras.Length; i++)
        {
            Camera cam = cameras[i];
            if (cam == null)
                continue;
            string target = cam.targetTexture != null ? cam.targetTexture.name : "(none)";
            Debug.Log("[LoginForest] camera name=" + cam.name +
                " enabled=" + cam.enabled +
                " depth=" + cam.depth.ToString("0.###") +
                " targetTexture=" + target +
                " cullingMask=0x" + cam.cullingMask.ToString("X8"));
        }

        Canvas[] canvases = UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int overlayCount = 0;
        int coverCount = 0;
        for (int i = 0; i < canvases.Length; i++)
        {
            Canvas canvas = canvases[i];
            if (canvas == null || canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                continue;
            overlayCount++;
            Debug.Log("[LoginForest] overlay canvas name=" + canvas.name +
                " enabled=" + canvas.enabled +
                " active=" + canvas.gameObject.activeInHierarchy);
            coverCount += LogCoveringGraphics(canvas);
        }

        Debug.Log("[LoginForest] overlay canvases=" + overlayCount + " coveringImages=" + coverCount);
        LogToolkitCovers();
    }

    static int LogCoveringGraphics(Canvas canvas)
    {
        int covers = 0;
        Image[] images = canvas.GetComponentsInChildren<Image>(true);
        for (int i = 0; i < images.Length; i++)
        {
            if (!CoversScreen(images[i].rectTransform))
                continue;
            covers++;
            string sprite = images[i].sprite != null ? images[i].sprite.name : "(none)";
            Debug.Log("[LoginForest] overlay image name=" + images[i].name +
                " sprite=" + sprite +
                " color=" + ColorRgb(images[i].color) +
                " enabled=" + images[i].enabled);
        }

        RawImage[] raws = canvas.GetComponentsInChildren<RawImage>(true);
        for (int i = 0; i < raws.Length; i++)
        {
            if (!CoversScreen(raws[i].rectTransform))
                continue;
            covers++;
            string texture = raws[i].texture != null ? raws[i].texture.name : "(none)";
            Debug.Log("[LoginForest] overlay raw name=" + raws[i].name +
                " texture=" + texture +
                " color=" + ColorRgb(raws[i].color) +
                " enabled=" + raws[i].enabled);
        }

        return covers;
    }

    static bool CoversScreen(RectTransform rect)
    {
        if (rect == null)
            return false;
        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);
        float minX = corners[0].x;
        float maxX = corners[0].x;
        float minY = corners[0].y;
        float maxY = corners[0].y;
        for (int i = 1; i < 4; i++)
        {
            minX = Mathf.Min(minX, corners[i].x);
            maxX = Mathf.Max(maxX, corners[i].x);
            minY = Mathf.Min(minY, corners[i].y);
            maxY = Mathf.Max(maxY, corners[i].y);
        }

        float sw = Mathf.Max(Screen.width, 1);
        float sh = Mathf.Max(Screen.height, 1);
        return (maxX - minX) >= sw * 0.9f && (maxY - minY) >= sh * 0.9f;
    }

    static void LogToolkitCovers()
    {
        UIDocument[] docs = UnityEngine.Object.FindObjectsByType<UIDocument>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < docs.Length; i++)
        {
            if (docs[i] == null || docs[i].rootVisualElement == null)
                continue;
            VisualElement root = docs[i].rootVisualElement;
            float panelW = root.resolvedStyle.width;
            float panelH = root.resolvedStyle.height;
            Debug.Log("[LoginForest] uitk document=" + docs[i].name +
                " enabled=" + docs[i].enabled +
                " panel=" + panelW.ToString("0") + "x" + panelH.ToString("0"));
            LogToolkitCover(root, panelW, panelH, 0);
        }
    }

    static void LogToolkitCover(VisualElement el, float panelW, float panelH, int depth)
    {
        if (el == null || depth > 12 || panelW < 1f || panelH < 1f)
            return;
        if (el.resolvedStyle.display == DisplayStyle.None)
            return;

        float w = el.resolvedStyle.width;
        float h = el.resolvedStyle.height;
        if (w >= panelW * 0.9f && h >= panelH * 0.9f)
        {
            Background bg = el.resolvedStyle.backgroundImage;
            Color color = el.resolvedStyle.backgroundColor;
            string image = BackgroundName(bg);
            if (image != "(none)" || color.a > 0.05f)
            {
                Debug.Log("[LoginForest] uitk cover name=" + el.name +
                    " image=" + image +
                    " color=" + ColorRgb(color) +
                    " alpha=" + color.a.ToString("0.###") +
                    " size=" + w.ToString("0") + "x" + h.ToString("0"));
            }
        }

        for (int i = 0; i < el.hierarchy.childCount; i++)
            LogToolkitCover(el.hierarchy[i], panelW, panelH, depth + 1);
    }

    static string BackgroundName(Background bg)
    {
        if (bg.texture != null)
            return bg.texture.name;
        if (bg.sprite != null)
            return bg.sprite.name;
        if (bg.renderTexture != null)
            return bg.renderTexture.name;
        if (bg.vectorImage != null)
            return bg.vectorImage.name;
        return "(none)";
    }

    static string ColorRgb(Color color)
    {
        return Mathf.RoundToInt(color.r * 255f) + "," +
            Mathf.RoundToInt(color.g * 255f) + "," +
            Mathf.RoundToInt(color.b * 255f);
    }

    static void WriteSceneOnlyProof(string path)
    {
        Camera[] cameras = UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Camera login = null;
        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i] != null && cameras[i].name == "Login")
            {
                login = cameras[i];
                break;
            }
        }

        if (login == null)
        {
            Debug.LogError("[LoginForest] no camera named Login; scene proof not written");
            return;
        }

        UIDocument[] docs = UnityEngine.Object.FindObjectsByType<UIDocument>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        bool[] docOn = new bool[docs.Length];
        for (int i = 0; i < docs.Length; i++)
        {
            docOn[i] = docs[i] != null && docs[i].enabled;
            if (docs[i] != null)
                docs[i].enabled = false;
        }

        Canvas[] canvases = UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        bool[] canvasOn = new bool[canvases.Length];
        for (int i = 0; i < canvases.Length; i++)
        {
            canvasOn[i] = canvases[i] != null && canvases[i].enabled;
            if (canvases[i] != null)
                canvases[i].enabled = false;
        }

        RenderTexture rt = null;
        Texture2D tex = null;
        RenderTexture prevTarget = login.targetTexture;
        RenderTexture prevActive = RenderTexture.active;
        int prevMask = login.cullingMask;
        try
        {
            int ui = LayerMask.NameToLayer("UI");
            if (ui >= 0)
                login.cullingMask &= ~(1 << ui);
            int w = login.pixelWidth > 16 ? login.pixelWidth : Mathf.Max(Screen.width, 1280);
            int h = login.pixelHeight > 16 ? login.pixelHeight : Mathf.Max(Screen.height, 720);
            rt = new RenderTexture(w, h, 24, RenderTextureFormat.ARGB32);
            rt.Create();
            login.targetTexture = rt;
            login.Render();
            RenderTexture.active = rt;
            tex = new Texture2D(w, h, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
            tex.Apply();
            byte[] png = tex.EncodeToPNG();
            string dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);
            File.WriteAllBytes(path, png);
            Debug.Log("[LoginForest] scene proof " + path + " bytes=" + png.Length +
                " camera=" + login.name + " " + w + "x" + h);
        }
        catch (Exception ex)
        {
            Debug.LogError("[LoginForest] scene proof failed: " + ex);
        }
        finally
        {
            login.targetTexture = prevTarget;
            login.cullingMask = prevMask;
            RenderTexture.active = prevActive;
            if (tex != null)
                UnityEngine.Object.Destroy(tex);
            if (rt != null)
            {
                rt.Release();
                UnityEngine.Object.Destroy(rt);
            }

            for (int i = 0; i < docs.Length; i++)
            {
                if (docs[i] != null)
                    docs[i].enabled = docOn[i];
            }

            for (int i = 0; i < canvases.Length; i++)
            {
                if (canvases[i] != null)
                    canvases[i].enabled = canvasOn[i];
            }
        }
    }

    // Same readiness contract the old presenter used: splash gone + LoginWindow
    // laid out wider than 200px.
    static bool LoginWindowShown()
    {
        UIDocument[] docs = UnityEngine.Object.FindObjectsByType<UIDocument>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        bool loginShown = false;
        for (int d = 0; d < docs.Length; d++)
        {
            if (docs[d] == null || docs[d].rootVisualElement == null)
                continue;
            VisualElement splash = docs[d].rootVisualElement.Q("InterludeSplash");
            if (splash != null && splash.parent != null)
                return false;
            VisualElement login = docs[d].rootVisualElement.Q("LoginWindow");
            float width = login != null ? login.resolvedStyle.width : 0f;
            if (login != null && login.resolvedStyle.display != DisplayStyle.None && width > 200f)
                loginShown = true;
        }
        return loginShown;
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
        // Batchmode / -quit proof runs must leave the process; interactive Editor stays open.
        if (Application.isBatchMode || IsCliProofRun())
            EditorApplication.Exit(code);
        else
            EditorApplication.isPlaying = false;
    }

    static bool IsCliProofRun()
    {
        string[] args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (string.Equals(args[i], "-fineage-login-forest-shot", System.StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
