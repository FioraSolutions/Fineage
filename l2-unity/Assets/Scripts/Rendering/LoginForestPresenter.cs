using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

/// <summary>
/// Login camera host. The vista itself is the baked l2_lobby tile plus
/// <see cref="L2LoginVista"/> (dump camera, zone fog, sun). This no longer
/// builds a side forest or overrides FOV.
/// </summary>
public sealed class LoginForestPresenter : MonoBehaviour
{
    const string ActorsResource = "Data/Maps/l2_lobby/Meta/LoginForestActors";
    public const string RootName = "LoginForest";
    public const string ShotPathFile = "/tmp/fineage-login-forest.outpath";

    const float JungleScroll = 0.0022f;

    struct FoliageSway
    {
        public Transform Transform;
        public Quaternion BaseLocal;
        public float Phase;
        public float AmpDeg;
        public float Speed;
    }

    static readonly string[] TextureRoots =
    {
        "Data/Textures/L2_Lobby_T",
        "Data/Textures/Primitive_FieldOBJ_T",
        "Data/Textures/field_deco_T",
        "Data/Textures/field_deco_S",
        "Data/Textures/oren_tree_t",
        "Data/Textures/speaking_tree_t",
        "Data/Textures/V_Obj_T",
        "Data/Textures/dion_tree_t",
        "Data/Textures/FX_E_T",
        "Data/Textures/lobby_terrain_t",
    };

    static readonly string[] ViewLayers =
    {
        "Default", "TransparentFX", "Terrain", "Water", "StaticMesh",
        "Brush", "Deco", "L2Sky", "L2Haze", "L2Clouds",
    };

    Camera _camera;
    bool _fogWas;
    Color _fogColorWas;
    FogMode _fogModeWas;
    float _fogStartWas;
    float _fogEndWas;
    float _fogDensityWas;
    Color _ambientWas;
    AmbientMode _ambientModeWas;
    Material _skyWas;
    bool _savedEnv;
    GameObject _sun;
    int _savedCullingMask;
    bool _savedCulling;
    CameraClearFlags _savedClearFlags;
    Color _savedBg;
    float _savedFov;
    float _savedNear;
    float _savedFar;
    Material _jungleMat;
    Material _terrainMat;
    int _drawLayer;
    bool _live;
    readonly List<Renderer> _hiddenLobby = new List<Renderer>(64);
    readonly List<FoliageSway> _sway = new List<FoliageSway>(256);

    public static void Present(Camera loginCamera)
    {
        if (loginCamera == null)
            return;
        LoginForestPresenter p = loginCamera.GetComponent<LoginForestPresenter>();
        if (p == null)
            p = loginCamera.gameObject.AddComponent<LoginForestPresenter>();
        p.Apply(loginCamera);
    }

    public static void DismissVistaOnly()
    {
        GameObject root = GameObject.Find(RootName);
        if (root != null)
            root.SetActive(false);

        LoginForestPresenter[] hosts = FindObjectsByType<LoginForestPresenter>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < hosts.Length; i++)
        {
            if (hosts[i] != null)
                hosts[i].RestoreVista();
        }
    }

    public static void Dismiss()
    {
        DismissVistaOnly();
        LoginForestMusic.Stop();
        LobbyAccountRenderSettings.Release();
        LoginForestPresenter[] hosts = FindObjectsByType<LoginForestPresenter>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < hosts.Length; i++)
        {
            if (hosts[i] != null)
                hosts[i].DeactivateLoginSun();
        }
    }

    void OnDisable()
    {
        Restore();
    }

    void LateUpdate()
    {
        if (!_live)
            return;

        // Subtle UV crawl on Jungle03 — retail lobby_back mist "breathes".
        if (_jungleMat != null)
        {
            Vector2 uv = _jungleMat.mainTextureOffset;
            uv.x += JungleScroll * Time.unscaledDeltaTime;
            _jungleMat.mainTextureOffset = uv;
            if (_jungleMat.HasProperty("_BaseMap"))
                _jungleMat.SetTextureOffset("_BaseMap", uv);
        }

        LoginForestMusic.TickVolume();
    }

    void Apply(Camera cam)
    {
        _camera = cam;
        Remember(cam);
        L2LoginVista.Apply(cam);
        _live = true;
        LoginForestMusic.Play();
        Debug.Log("[LoginForest] vista fov=" + cam.fieldOfView + " actor=" + cam.transform.position);

        if (File.Exists(ShotPathFile))
            StartCoroutine(CaptureProof());
    }

    void RestoreHiddenLobby()
    {
        for (int i = 0; i < _hiddenLobby.Count; i++)
        {
            if (_hiddenLobby[i] != null)
                _hiddenLobby[i].enabled = true;
        }

        _hiddenLobby.Clear();
    }

    void PushEnv()
    {
        LobbyAccountRenderSettings.Apply();
        _savedEnv = true;
        if (_sun != null)
            _sun.SetActive(true);
    }

    void PopEnv()
    {
        if (!_savedEnv)
            return;
        LobbyAccountRenderSettings.Release();
        if (_sun != null)
            _sun.SetActive(false);
        _savedEnv = false;
    }

    void Remember(Camera cam)
    {
        if (_savedCulling)
            return;
        _savedCullingMask = cam.cullingMask;
        _savedClearFlags = cam.clearFlags;
        _savedBg = cam.backgroundColor;
        _savedFov = cam.fieldOfView;
        _savedNear = cam.nearClipPlane;
        _savedFar = cam.farClipPlane;
        _savedCulling = true;
    }

    void RestoreVista()
    {
        L2LoginVista.Restore();
        _live = false;
        _sway.Clear();
        RestoreHiddenLobby();

        LoginJungleFillPass pass = _camera != null ? _camera.GetComponent<LoginJungleFillPass>() : null;
        if (pass != null)
            Destroy(pass);

        if (_camera != null)
        {
            Transform card = _camera.transform.Find("LoginJungleCard");
            if (card != null)
                Destroy(card.gameObject);
            Transform screenGround = _camera.transform.Find("LoginScreenGround");
            if (screenGround != null)
                Destroy(screenGround.gameObject);
        }

        if (_jungleMat != null)
        {
            Destroy(_jungleMat);
            _jungleMat = null;
        }

        _terrainMat = null;

        if (_camera == null || !_savedCulling)
            return;
        _camera.cullingMask = _savedCullingMask;
        _camera.clearFlags = _savedClearFlags;
        _camera.backgroundColor = _savedBg;
        _camera.fieldOfView = _savedFov;
        _camera.nearClipPlane = _savedNear;
        _camera.farClipPlane = _savedFar;
        _savedCulling = false;
    }

    void DeactivateLoginSun()
    {
        if (_sun != null)
            _sun.SetActive(false);
        _savedEnv = false;
    }

    void Restore()
    {
        RestoreVista();
        LoginForestMusic.Stop();
        PopEnv();
    }

    IEnumerator CaptureProof()
    {
        string path;
        try
        {
            path = File.ReadAllText(ShotPathFile).Trim();
        }
        catch (IOException)
        {
            yield break;
        }

        if (string.IsNullOrEmpty(path))
            yield break;

        bool loginShown = false;
        for (int i = 0; i < 600; i++)
        {
            UIDocument[] docs = FindObjectsByType<UIDocument>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            bool splashGone = true;
            loginShown = false;
            for (int d = 0; d < docs.Length; d++)
            {
                if (docs[d] == null || docs[d].rootVisualElement == null)
                    continue;
                VisualElement splash = docs[d].rootVisualElement.Q("InterludeSplash");
                if (splash != null && splash.parent != null)
                    splashGone = false;
                VisualElement login = docs[d].rootVisualElement.Q("LoginWindow");
                float width = login != null ? login.resolvedStyle.width : 0f;
                if (login != null && login.resolvedStyle.display != DisplayStyle.None && width > 200f)
                    loginShown = true;
            }

            if (splashGone && loginShown)
                break;
            yield return null;
        }

        if (!loginShown)
            Debug.LogError("[LoginForest] LoginWindow was not up after 600 frames; writing the vista shot anyway");

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        Texture2D shot = ScreenCapture.CaptureScreenshotAsTexture();
        if (shot == null)
        {
            Debug.LogError("[LoginForest] ScreenCapture returned null; no proof shot written to " + path);
            yield break;
        }
        byte[] png = shot.EncodeToPNG();
        Destroy(shot);
        string dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
        File.WriteAllBytes(path, png);
        Debug.Log("[LoginForest] wrote " + path);
    }

    [Serializable]
    class LoginForestFile
    {
        public LoginForestVec camera;
        public LoginForestFog fog;
        public LoginForestActor[] actors;
    }

    [Serializable]
    class LoginForestVec
    {
        public float x, y, z, pitch, yaw, roll, fov;
    }

    [Serializable]
    class LoginForestFog
    {
        public float r, g, b, startUu, endUu;
    }

    [Serializable]
    class LoginForestActor
    {
        public string mesh;
        public float x, y, z, pitch, yaw, roll, sx, sy, sz;
    }
}
