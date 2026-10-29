using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

/// <summary>
/// Shows the Interlude Lobby.unr login vista (forest path) on the credentials camera.
/// Placements come from Meta/LoginForestActors.json. Harbor geometry stays on other layers
/// so character select is unchanged.
/// </summary>
public sealed class LoginForestPresenter : MonoBehaviour
{
    const string ActorsResource = "Data/Maps/l2_lobby/Meta/LoginForestActors";
    public const string ShotPathFile = "/tmp/fineage-login-forest.outpath";
    const int LoginLayer = 17;
    const float Fov = 62f;
    const float BackdropDistance = 90f;
    const float FogEndMeters = 420f;

    static readonly string[] TextureRoots =
    {
        "Data/Textures/L2_Lobby_T",
        "Data/Textures/Primitive_FieldOBJ_T",
        "Data/Textures/field_deco_T",
        "Data/Textures/field_deco_S",
    };

    Camera _camera;
    bool _fogWas;
    Color _fogColorWas;
    FogMode _fogModeWas;
    float _fogStartWas;
    float _fogEndWas;
    Color _ambientWas;
    AmbientMode _ambientModeWas;
    bool _savedEnv;
    GameObject _sun;

    public static void Present(Camera loginCamera)
    {
        if (loginCamera == null)
            return;
        LoginForestPresenter presenter = loginCamera.GetComponent<LoginForestPresenter>();
        if (presenter == null)
            presenter = loginCamera.gameObject.AddComponent<LoginForestPresenter>();
        presenter.Apply(loginCamera);
    }

    void OnEnable()
    {
        if (_camera != null)
            PushAtmosphere();
    }

    void OnDisable()
    {
        PopAtmosphere();
    }

    void Apply(Camera loginCamera)
    {
        _camera = loginCamera;
        TextAsset json = Resources.Load<TextAsset>(ActorsResource);
        if (json == null)
        {
            Debug.LogWarning("[LoginForest] missing " + ActorsResource);
            return;
        }

        LoginForestFile file = JsonUtility.FromJson<LoginForestFile>(json.text);
        if (file == null || file.camera == null)
            return;

        Vector3 pos = VectorUtils.ConvertPosToUnity(new Vector3(file.camera.x, file.camera.y, file.camera.z));
        Vector3 euler = VectorUtils.ConvertRotToUnity(new Vector3(file.camera.pitch, file.camera.yaw, file.camera.roll));
        Transform cam = loginCamera.transform;
        cam.SetPositionAndRotation(pos, Quaternion.Euler(euler));
        loginCamera.fieldOfView = Fov;
        loginCamera.nearClipPlane = 0.15f;
        loginCamera.farClipPlane = 450f;
        loginCamera.clearFlags = CameraClearFlags.SolidColor;
        loginCamera.backgroundColor = FogColor(file);
        loginCamera.allowHDR = false;
        loginCamera.useOcclusionCulling = false;
        loginCamera.cullingMask = 1 << LoginLayer;

        HideForestFromOtherCameras(loginCamera);
        BuildOnce(file);
        PushAtmosphere(file);
        if (File.Exists(ShotPathFile))
            StartCoroutine(CaptureProof());
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

        bool ready = false;
        for (int i = 0; i < 2400; i++)
        {
            UIDocument[] docs = FindObjectsByType<UIDocument>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            bool splashGone = true;
            bool loginShown = false;
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
            {
                ready = true;
                break;
            }

            yield return null;
        }

        if (!ready)
            Debug.LogWarning("[LoginForest] credentials window was not ready for the proof shot");

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        Texture2D tex = ScreenCapture.CaptureScreenshotAsTexture();
        if (tex == null)
        {
            Debug.LogWarning("[LoginForest] screenshot capture returned null");
            yield break;
        }

        byte[] png = tex.EncodeToPNG();
        Destroy(tex);
        string dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
        File.WriteAllBytes(path, png);
        Debug.Log("[LoginForest] wrote " + path + " bytes=" + png.Length);
    }

    static Color FogColor(LoginForestFile file)
    {
        if (file.fog == null)
            return new Color(0.55f, 0.68f, 0.72f, 1f);
        return new Color(file.fog.r / 255f, file.fog.g / 255f, file.fog.b / 255f, 1f);
    }

    static void HideForestFromOtherCameras(Camera loginCamera)
    {
        int bit = 1 << LoginLayer;
        Camera[] cameras = FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i] == null || cameras[i] == loginCamera)
                continue;
            cameras[i].cullingMask &= ~bit;
        }
    }

    void BuildOnce(LoginForestFile file)
    {
        if (GameObject.Find("LoginForest") != null)
            return;

        var root = new GameObject("LoginForest");
        SetLayer(root);

        Dictionary<string, Texture2D> textures = LoadTextures();
        Dictionary<string, Material> materials = new Dictionary<string, Material>(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, GameObject> meshes = new Dictionary<string, GameObject>(StringComparer.OrdinalIgnoreCase);
        int placed = 0;
        int missing = 0;

        LoginForestActor[] actors = file.actors ?? Array.Empty<LoginForestActor>();
        for (int i = 0; i < actors.Length; i++)
        {
            LoginForestActor actor = actors[i];
            if (actor == null || string.IsNullOrEmpty(actor.mesh))
                continue;

            string[] parts = actor.mesh.Split('.');
            if (parts.Length < 2)
                continue;
            string package = parts[0];
            string meshName = parts[parts.Length - 1];
            string key = package + "/" + meshName;
            if (!meshes.TryGetValue(key, out GameObject prefab))
            {
                prefab = Resources.Load<GameObject>("Data/StaticMeshes/" + package + "/" + meshName);
                meshes[key] = prefab;
            }

            if (prefab == null)
            {
                missing++;
                continue;
            }

            Vector3 position = VectorUtils.ConvertPosToUnity(new Vector3(actor.x, actor.y, actor.z));
            Vector3 rotation = VectorUtils.ConvertRotToUnity(new Vector3(actor.pitch, actor.yaw, actor.roll));
            float sx = actor.sx == 0f ? 1f : actor.sx;
            float sy = actor.sy == 0f ? 1f : actor.sy;
            float sz = actor.sz == 0f ? 1f : actor.sz;

            GameObject instance = Instantiate(prefab, position, Quaternion.Euler(rotation), root.transform);
            instance.name = meshName;
            instance.transform.localScale = new Vector3(sx, sy, sz);
            SetLayer(instance);
            bool unlit = meshName.IndexOf("lobby_back", StringComparison.OrdinalIgnoreCase) >= 0
                || meshName.IndexOf("sky", StringComparison.OrdinalIgnoreCase) >= 0;
            AssignMaterials(instance, textures, materials, unlit);
            placed++;
        }

        BuildBackdrop(root, textures, materials);

        _sun = new GameObject("LoginForestSun");
        _sun.transform.SetParent(root.transform, false);
        _sun.transform.rotation = Quaternion.Euler(48f, -30f, 0f);
        Light light = _sun.AddComponent<Light>();
        light.type = LightType.Directional;
        light.color = new Color(0.78f, 0.88f, 0.95f);
        light.intensity = 0.85f;
        light.shadows = LightShadows.None;
        SetLayer(_sun);

        Debug.Log("[LoginForest] placed=" + placed + " missing=" + missing + " textures=" + textures.Count);
    }

    /// <summary>
    /// Camera-anchored jungle panorama so the void past the actors reads as forest,
    /// not the white fog colour (Lobby.unr hides it behind a skybox zone).
    /// </summary>
    void BuildBackdrop(GameObject root, Dictionary<string, Texture2D> textures, Dictionary<string, Material> cache)
    {
        if (_camera == null)
            return;
        Material mat = MaterialFor("Jungle03", textures, cache, true);
        if (mat.mainTexture == null)
            return;
        var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.name = "LoginForestBackdrop";
        Destroy(quad.GetComponent<Collider>());
        quad.transform.SetParent(_camera.transform, false);
        float h = 2f * BackdropDistance * Mathf.Tan(Fov * 0.5f * Mathf.Deg2Rad) * 1.15f;
        quad.transform.localPosition = new Vector3(0f, 0f, BackdropDistance);
        quad.transform.localRotation = Quaternion.identity;
        quad.transform.localScale = new Vector3(h * 2f, h, 1f);
        Renderer r = quad.GetComponent<Renderer>();
        r.sharedMaterial = mat;
        r.shadowCastingMode = ShadowCastingMode.Off;
        r.receiveShadows = false;
        SetLayer(quad);
    }

    static void AssignMaterials(GameObject instance, Dictionary<string, Texture2D> textures, Dictionary<string, Material> cache, bool unlit)
    {
        Renderer[] renderers = instance.GetComponentsInChildren<Renderer>(true);
        for (int r = 0; r < renderers.Length; r++)
        {
            Material[] slots = renderers[r].sharedMaterials;
            var next = new Material[slots.Length];
            for (int s = 0; s < slots.Length; s++)
            {
                string matName = slots[s] != null ? slots[s].name : string.Empty;
                next[s] = MaterialFor(matName, textures, cache, unlit);
            }

            renderers[r].sharedMaterials = next;
            renderers[r].shadowCastingMode = unlit ? ShadowCastingMode.Off : ShadowCastingMode.On;
            renderers[r].receiveShadows = !unlit;
        }
    }

    static Material MaterialFor(string materialName, Dictionary<string, Texture2D> textures, Dictionary<string, Material> cache, bool unlit)
    {
        string texName = TextureKey(materialName);
        string cacheKey = (unlit ? "u:" : "l:") + texName;
        if (cache.TryGetValue(cacheKey, out Material existing))
            return existing;

        Texture2D texture = FindTexture(textures, texName);
        bool cutout = !unlit && WantsCutout(texName);
        Shader shader = Shader.Find(unlit
            ? "Universal Render Pipeline/Unlit"
            : "Universal Render Pipeline/Lit");
        var material = new Material(shader != null ? shader : Shader.Find("Sprites/Default"));
        material.name = string.IsNullOrEmpty(texName) ? "login-forest" : texName;
        if (texture != null)
        {
            material.mainTexture = texture;
            if (material.HasProperty("_BaseMap"))
                material.SetTexture("_BaseMap", texture);
        }

        if (material.HasProperty("_Cull"))
            material.SetFloat("_Cull", 0f);
        if (material.HasProperty("_Smoothness"))
            material.SetFloat("_Smoothness", 0f);
        if (material.HasProperty("_BaseColor"))
            material.SetColor("_BaseColor", Color.white);

        if (cutout)
        {
            if (material.HasProperty("_AlphaClip"))
                material.SetFloat("_AlphaClip", 1f);
            material.EnableKeyword("_ALPHATEST_ON");
            if (material.HasProperty("_Cutoff"))
                material.SetFloat("_Cutoff", 0.4f);
            material.SetOverrideTag("RenderType", "TransparentCutout");
            material.renderQueue = (int)RenderQueue.AlphaTest;
        }

        cache[cacheKey] = material;
        return material;
    }

    static bool WantsCutout(string name)
    {
        if (string.IsNullOrEmpty(name))
            return false;
        string n = name.ToLowerInvariant();
        return n.Contains("leaf") || n.Contains("grass") || n.Contains("flower")
            || n.Contains("deco") || n.Contains("fern");
    }

    static string TextureKey(string materialName)
    {
        if (string.IsNullOrEmpty(materialName))
            return string.Empty;
        string name = materialName;
        int space = name.IndexOf(' ');
        if (space > 0)
            name = name.Substring(0, space);
        int dot = name.LastIndexOf('.');
        if (dot >= 0 && dot < name.Length - 1)
            name = name.Substring(dot + 1);
        if (name.EndsWith("_1", StringComparison.Ordinal) || name.EndsWith("_2", StringComparison.Ordinal))
            name = name.Substring(0, name.Length - 2);
        return name;
    }

    static Texture2D FindTexture(Dictionary<string, Texture2D> textures, string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;
        if (textures.TryGetValue(name, out Texture2D tex))
            return tex;
        string lower = name.ToLowerInvariant();
        foreach (KeyValuePair<string, Texture2D> pair in textures)
        {
            if (string.Equals(pair.Key, lower, StringComparison.OrdinalIgnoreCase))
                return pair.Value;
        }

        return null;
    }

    static Dictionary<string, Texture2D> LoadTextures()
    {
        var map = new Dictionary<string, Texture2D>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < TextureRoots.Length; i++)
        {
            Texture2D[] found = Resources.LoadAll<Texture2D>(TextureRoots[i]);
            for (int t = 0; t < found.Length; t++)
            {
                if (found[t] != null && !map.ContainsKey(found[t].name))
                    map.Add(found[t].name, found[t]);
            }
        }

        return map;
    }

    void PushAtmosphere(LoginForestFile file = null)
    {
        if (!_savedEnv)
        {
            _fogWas = RenderSettings.fog;
            _fogColorWas = RenderSettings.fogColor;
            _fogModeWas = RenderSettings.fogMode;
            _fogStartWas = RenderSettings.fogStartDistance;
            _fogEndWas = RenderSettings.fogEndDistance;
            _ambientWas = RenderSettings.ambientLight;
            _ambientModeWas = RenderSettings.ambientMode;
            _savedEnv = true;
        }

        float start = 9.5f;
        float end = FogEndMeters;
        Color color = _camera != null ? _camera.backgroundColor : new Color(0.77f, 0.82f, 0.82f);
        if (file != null && file.fog != null)
        {
            start = file.fog.startUu / 52.5f;
            end = Mathf.Max(file.fog.endUu / 52.5f, FogEndMeters);
            color = FogColor(file);
        }

        // Linear fog on URP Unlit washes the jungle panorama to the fog colour (white void); the
        // painted backdrop already carries the Lobby.unr haze.
        RenderSettings.fog = false;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogColor = color;
        RenderSettings.fogStartDistance = start;
        RenderSettings.fogEndDistance = end;
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.42f, 0.52f, 0.56f);
        if (_sun != null)
            _sun.SetActive(true);
    }

    void PopAtmosphere()
    {
        if (!_savedEnv)
            return;
        RenderSettings.fog = _fogWas;
        RenderSettings.fogColor = _fogColorWas;
        RenderSettings.fogMode = _fogModeWas;
        RenderSettings.fogStartDistance = _fogStartWas;
        RenderSettings.fogEndDistance = _fogEndWas;
        RenderSettings.ambientMode = _ambientModeWas;
        RenderSettings.ambientLight = _ambientWas;
        if (_sun != null)
            _sun.SetActive(false);
    }

    static void SetLayer(GameObject go)
    {
        Transform[] transforms = go.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < transforms.Length; i++)
            transforms[i].gameObject.layer = LoginLayer;
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
        public float x;
        public float y;
        public float z;
        public float pitch;
        public float yaw;
        public float roll;
    }

    [Serializable]
    class LoginForestFog
    {
        public float r;
        public float g;
        public float b;
        public float startUu;
        public float endUu;
    }

    [Serializable]
    class LoginForestActor
    {
        public string mesh;
        public float x;
        public float y;
        public float z;
        public float pitch;
        public float yaw;
        public float roll;
        public float sx;
        public float sy;
        public float sz;
    }
}
