using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

/// <summary>
/// Interlude login vista — Lobby.unr Camera25 stack (no screen-space path/card hacks):
/// Camera25 FOV 35 → <c>lobby_back</c> + Jungle03 → additive lobby terrain → static meshes.
/// Foliage sway + intro BGM (<see cref="LoginForestMusic"/>). Fog off (mist is in Jungle03).
/// </summary>
public sealed class LoginForestPresenter : MonoBehaviour
{
    const string ActorsResource = "Data/Maps/l2_lobby/Meta/LoginForestActors";
    public const string RootName = "LoginForest";
    public const string ShotPathFile = "/tmp/fineage-login-forest.outpath";

    const float MeshLocalYawFix = 90f;
    const float Fov = 35f;
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

    public static void Dismiss()
    {
        GameObject root = GameObject.Find(RootName);
        if (root != null)
            root.SetActive(false);

        LoginForestPresenter[] hosts = FindObjectsByType<LoginForestPresenter>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < hosts.Length; i++)
        {
            if (hosts[i] != null)
                hosts[i].Restore();
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

        TickFoliageSway();
        LoginForestMusic.TickVolume();
    }

    void Apply(Camera cam)
    {
        _camera = cam;
        TextAsset json = Resources.Load<TextAsset>(ActorsResource);
        if (json == null)
        {
            Debug.LogWarning("[LoginForest] missing " + ActorsResource);
            return;
        }

        LoginForestFile file = JsonUtility.FromJson<LoginForestFile>(json.text);
        if (file == null || file.camera == null)
            return;

        Remember(cam);

        Vector3 pos = VectorUtils.ConvertPosToUnity(new Vector3(file.camera.x, file.camera.y, file.camera.z));
        Vector3 euler = VectorUtils.ConvertRotToUnity(new Vector3(file.camera.pitch, file.camera.yaw, file.camera.roll));
        cam.transform.SetPositionAndRotation(pos, Quaternion.Euler(euler));
        cam.fieldOfView = Fov;
        cam.nearClipPlane = 0.1f;
        cam.farClipPlane = 280f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.28f, 0.40f, 0.44f, 1f);
        cam.allowHDR = false;
        cam.useOcclusionCulling = false;
        cam.cullingMask = BuildMask();
        _drawLayer = LayerMask.NameToLayer("StaticMesh");
        if (_drawLayer < 0)
            _drawLayer = 0;
        _live = true;

        HideAdditiveLobbyJunk();
        BuildForest(file);

        GameObject root = GameObject.Find(RootName);
        if (root != null)
            root.SetActive(true);

        // Retail SoT: Lobby.unr lobby_back mesh textured with Jungle03 — not a camera quad.
        PlaceLobbyBack(file, root);
        PushEnv();
        cam.clearFlags = CameraClearFlags.SolidColor;
        // Jungle03 mid mist — only shows in rare gaps outside lobby_back extents.
        cam.backgroundColor = new Color(0.24f, 0.34f, 0.39f, 1f);

        LoginForestMusic.Play();
        Debug.Log("[LoginForest] ready fov=" + Fov
            + " lobby_back=" + (root != null && root.transform.Find("lobby_back") != null)
            + " jungleMat=" + (_jungleMat != null)
            + " sway=" + _sway.Count);

        if (File.Exists(ShotPathFile))
            StartCoroutine(CaptureProof());
    }

    void PlaceLobbyBack(LoginForestFile file, GameObject root)
    {
        if (file?.actors == null || root == null)
            return;

        LoginForestActor back = null;
        for (int i = 0; i < file.actors.Length; i++)
        {
            LoginForestActor a = file.actors[i];
            if (a != null && !string.IsNullOrEmpty(a.mesh)
                && a.mesh.IndexOf("lobby_back", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                back = a;
                break;
            }
        }

        if (back == null)
            return;

        GameObject prefab = Resources.Load<GameObject>("Data/StaticMeshes/L2_Lobby/lobby_back");
        if (prefab == null)
            return;

        Texture2D tex = Resources.Load<Texture2D>("Data/Textures/L2_Lobby_T/Jungle03");
        Vector3 position = VectorUtils.ConvertPosToUnity(new Vector3(back.x, back.y, back.z));
        Vector3 rotation = VectorUtils.ConvertRotToUnity(new Vector3(back.pitch, back.yaw, back.roll));
        float sx = back.sx == 0f ? 1f : back.sx;
        float sy = back.sy == 0f ? 1f : back.sy;
        float sz = back.sz == 0f ? 1f : back.sz;
        Quaternion meshLocal = Quaternion.Euler(0f, MeshLocalYawFix, 0f) * prefab.transform.rotation;
        Quaternion worldRot = Quaternion.Euler(rotation) * meshLocal;
        Vector3 scale = RemapDrawScale(sx, sy, sz, meshLocal);

        Transform old = root.transform.Find("lobby_back");
        if (old != null)
            Destroy(old.gameObject);

        GameObject instance = Instantiate(prefab, position, worldRot, root.transform);
        instance.name = "lobby_back";
        instance.transform.localScale = Vector3.Scale(prefab.transform.localScale, scale);
        SetLayer(instance, _drawLayer);

        if (_jungleMat != null)
            Destroy(_jungleMat);
        _jungleMat = new Material(FindUnlit()) { name = "login-lobby-back-jungle03" };
        if (tex != null)
            ApplyTexture(_jungleMat, tex, Vector2.one);
        if (_jungleMat.HasProperty("_BaseColor"))
            _jungleMat.SetColor("_BaseColor", Color.white);
        if (_jungleMat.HasProperty("_Color"))
            _jungleMat.SetColor("_Color", Color.white);
        _jungleMat.renderQueue = (int)RenderQueue.Geometry - 20;
        if (_jungleMat.HasProperty("_Cull"))
            _jungleMat.SetFloat("_Cull", 0f);
        if (_jungleMat.HasProperty("_ZWrite"))
            _jungleMat.SetFloat("_ZWrite", 1f);

        Renderer[] rends = instance.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < rends.Length; i++)
        {
            var slots = new Material[Mathf.Max(1, rends[i].sharedMaterials.Length)];
            for (int s = 0; s < slots.Length; s++)
                slots[s] = _jungleMat;
            rends[i].sharedMaterials = slots;
            rends[i].shadowCastingMode = ShadowCastingMode.Off;
            rends[i].receiveShadows = false;
            rends[i].enabled = true;
        }

        Debug.Log("[LoginForest] lobby_back at " + position + " scale=" + instance.transform.localScale
            + " jungle=" + (tex != null));
    }

    void HideAdditiveLobbyJunk()
    {
        RestoreHiddenLobby();
        int terrainLayer = LayerMask.NameToLayer("Terrain");
        int brushLayer = LayerMask.NameToLayer("Brush");
        Renderer[] rends = FindObjectsByType<Renderer>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        for (int i = 0; i < rends.Length; i++)
        {
            Renderer r = rends[i];
            if (r == null || !r.enabled)
                continue;

            int layer = r.gameObject.layer;
            if (layer == terrainLayer || layer == brushLayer)
            {
                // Path under Camera25 lives on additive lobby terrain — keep it, force unlit dirt.
                ForceUnlitDirt(r);
                continue;
            }

            string goName = r.gameObject.name;
            if (string.Equals(goName, "l2_lobby", StringComparison.OrdinalIgnoreCase)
                || goName.IndexOf("Terrain", StringComparison.OrdinalIgnoreCase) >= 0
                || goName.IndexOf("l2_lobby_Mesh", StringComparison.OrdinalIgnoreCase) >= 0
                || goName.IndexOf("Alphamap", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                ForceUnlitDirt(r);
                continue;
            }

            Transform t = r.transform;
            bool underLobby = false;
            while (t != null)
            {
                string n = t.name;
                if (string.Equals(n, "l2_lobby", StringComparison.OrdinalIgnoreCase)
                    || n.IndexOf("SceneRoots", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    underLobby = true;
                    break;
                }

                t = t.parent;
            }

            if (!underLobby)
                continue;

            // Ships / sky / FX from additive lobby — hide. Terrain kept above.
            string low = goName.ToLowerInvariant();
            bool keep = low.Contains("terrain") || low.Contains("brush") || low.Contains("floor");
            if (keep)
            {
                ForceUnlitDirt(r);
                continue;
            }

            r.enabled = false;
            _hiddenLobby.Add(r);
        }

        if (_hiddenLobby.Count > 0)
            Debug.Log("[LoginForest] hid additive lobby renderers=" + _hiddenLobby.Count);
    }

    void ForceUnlitDirt(Renderer r)
    {
        if (r == null)
            return;

        if (_terrainMat == null)
        {
            // Real Interlude albedo (lobby_terrain_t/D_M_* are near-black modulate leftovers).
            Texture2D dirt = Resources.Load<Texture2D>("Data/Textures/field_deco_T/beachstone1")
                ?? Resources.Load<Texture2D>("Data/Textures/field_deco_T/Decostone06")
                ?? Resources.Load<Texture2D>("Data/Textures/field_deco_T/dgstoneA1");
            _terrainMat = new Material(FindUnlit()) { name = "login-terrain-dirt" };
            if (dirt != null)
                ApplyTexture(_terrainMat, dirt, new Vector2(12f, 12f));
            // Muddy path tint over stone albedo — matches Interlude Camera25 trail.
            Color tint = new Color(0.42f, 0.38f, 0.26f, 1f);
            if (_terrainMat.HasProperty("_BaseColor"))
                _terrainMat.SetColor("_BaseColor", tint);
            if (_terrainMat.HasProperty("_Color"))
                _terrainMat.SetColor("_Color", tint);
            _terrainMat.renderQueue = (int)RenderQueue.Geometry - 5;
        }

        var slots = new Material[Mathf.Max(1, r.sharedMaterials.Length)];
        for (int i = 0; i < slots.Length; i++)
            slots[i] = _terrainMat;
        r.sharedMaterials = slots;
        r.shadowCastingMode = ShadowCastingMode.Off;
        r.receiveShadows = false;
        r.enabled = true;
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

    void BuildForest(LoginForestFile file)
    {
        GameObject existing = GameObject.Find(RootName);
        if (existing != null)
        {
            existing.name = RootName + "_old";
            Destroy(existing);
        }

        if (_sun != null)
        {
            Destroy(_sun);
            _sun = null;
        }

        _sway.Clear();
        int layer = _drawLayer;

        var root = new GameObject(RootName);
        SetLayer(root, layer);

        Dictionary<string, Texture2D> textures = LoadTextures();
        var materials = new Dictionary<string, Material>(StringComparer.OrdinalIgnoreCase);
        var meshes = new Dictionary<string, GameObject>(StringComparer.OrdinalIgnoreCase);
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
            if (meshName.IndexOf("lobby_back", StringComparison.OrdinalIgnoreCase) >= 0
                || meshName.IndexOf("sky", StringComparison.OrdinalIgnoreCase) >= 0)
                continue;

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

            Quaternion meshLocal = Quaternion.Euler(0f, MeshLocalYawFix, 0f) * prefab.transform.rotation;
            Quaternion worldRot = Quaternion.Euler(rotation) * meshLocal;
            Vector3 scale = RemapDrawScale(sx, sy, sz, meshLocal);

            GameObject instance = Instantiate(prefab, position, worldRot, root.transform);
            instance.name = meshName;
            instance.transform.localScale = Vector3.Scale(prefab.transform.localScale, scale);
            SetLayer(instance, layer);
            AssignMaterials(instance, textures, materials);
            RegisterFoliageSway(instance, meshName);
            placed++;
        }

        _sun = new GameObject("LoginForestSun");
        _sun.transform.SetParent(root.transform, false);
        _sun.transform.rotation = Quaternion.Euler(42f, -30f, 0f);
        Light light = _sun.AddComponent<Light>();
        light.type = LightType.Directional;
        light.color = new Color(0.70f, 0.82f, 0.90f);
        light.intensity = 0.55f;
        light.shadows = LightShadows.None;
        SetLayer(_sun, layer);

        Debug.Log("[LoginForest] placed=" + placed + " missing=" + missing + " textures=" + textures.Count);
    }

    static void AssignMaterials(
        GameObject instance,
        Dictionary<string, Texture2D> textures,
        Dictionary<string, Material> cache)
    {
        Renderer[] renderers = instance.GetComponentsInChildren<Renderer>(true);
        for (int r = 0; r < renderers.Length; r++)
        {
            Material[] slots = renderers[r].sharedMaterials;
            var next = new Material[slots.Length];
            for (int s = 0; s < slots.Length; s++)
            {
                string matName = slots[s] != null ? slots[s].name : string.Empty;
                next[s] = MaterialFor(matName, textures, cache);
            }

            renderers[r].sharedMaterials = next;
            renderers[r].shadowCastingMode = ShadowCastingMode.Off;
            renderers[r].receiveShadows = false;
        }
    }

    static Material MaterialFor(
        string materialName,
        Dictionary<string, Texture2D> textures,
        Dictionary<string, Material> cache)
    {
        string texName = TextureKey(materialName);
        if (cache.TryGetValue(texName, out Material existing))
            return existing;

        Texture2D texture = FindTexture(textures, texName);
        bool cutout = WantsCutout(texName);
        // Unlit for foliage — Lit + bright ambient made leaves look pasted/oversharp vs retail.
        Shader shader = cutout ? FindUnlit() : (Shader.Find("Universal Render Pipeline/Lit") ?? FindUnlit());
        var material = new Material(shader) { name = string.IsNullOrEmpty(texName) ? "login-forest" : texName };
        if (texture != null)
            ApplyTexture(material, texture, Vector2.one);

        if (material.HasProperty("_Cull"))
            material.SetFloat("_Cull", 0f);
        if (material.HasProperty("_Smoothness"))
            material.SetFloat("_Smoothness", 0f);
        if (material.HasProperty("_Metallic"))
            material.SetFloat("_Metallic", 0f);

        Color tint = cutout
            ? new Color(0.40f, 0.48f, 0.36f, 1f)
            : new Color(0.70f, 0.72f, 0.66f, 1f);
        if (material.HasProperty("_BaseColor"))
            material.SetColor("_BaseColor", tint);
        if (material.HasProperty("_Color"))
            material.SetColor("_Color", tint);

        if (cutout)
        {
            if (material.HasProperty("_AlphaClip"))
                material.SetFloat("_AlphaClip", 1f);
            material.EnableKeyword("_ALPHATEST_ON");
            // Softer than retail UE2 mask still, but lower cutoff = less crunchy white edges.
            if (material.HasProperty("_Cutoff"))
                material.SetFloat("_Cutoff", 0.18f);
            material.SetOverrideTag("RenderType", "TransparentCutout");
            material.renderQueue = (int)RenderQueue.AlphaTest;
        }

        cache[texName] = material;
        return material;
    }

    static bool WantsCutout(string name)
    {
        if (string.IsNullOrEmpty(name))
            return false;
        string n = name.ToLowerInvariant();
        return n.Contains("leaf") || n.Contains("grass") || n.Contains("flower")
            || n.Contains("fern") || n.Contains("bush") || n.Contains("vine")
            || n.Contains("plant") || n.Contains("moss") || n.Contains("ivy");
    }

    void RegisterFoliageSway(GameObject instance, string meshName)
    {
        if (!WantsSway(meshName))
            return;

        float amp;
        float speed;
        string n = meshName.ToLowerInvariant();
        // Amps readable in Game view — retail AmbientMesh wind is obvious on crowns/ferns.
        if (n.Contains("tree") || n.Contains("trunk") || n.Contains("intree") || n.Contains("deco000"))
        {
            amp = 2.4f;
            speed = 0.48f;
        }
        else if (n.Contains("fern") || n.Contains("giantgrass") || n.Contains("gardengrass"))
        {
            amp = 5.5f;
            speed = 0.85f;
        }
        else if (n.Contains("grass") || n.Contains("flower") || n.Contains("bush") || n.Contains("vine"))
        {
            amp = 4.2f;
            speed = 1.05f;
        }
        else
        {
            amp = 3.0f;
            speed = 0.70f;
        }

        _sway.Add(new FoliageSway
        {
            Transform = instance.transform,
            BaseLocal = instance.transform.localRotation,
            Phase = (meshName.GetHashCode() * 0.0001f) + (_sway.Count * 0.37f),
            AmpDeg = amp,
            Speed = speed,
        });
    }

    static bool WantsSway(string meshName)
    {
        if (string.IsNullOrEmpty(meshName))
            return false;
        string n = meshName.ToLowerInvariant();
        if (n.Contains("stone") || n.Contains("statue") || n.Contains("rock") || n.Contains("mushroom"))
            return false;
        return n.Contains("tree") || n.Contains("leaf") || n.Contains("grass") || n.Contains("fern")
            || n.Contains("bush") || n.Contains("vine") || n.Contains("plant") || n.Contains("flower")
            || n.Contains("moss") || n.Contains("ivy") || n.Contains("intree") || n.Contains("decotree");
    }

    void TickFoliageSway()
    {
        float t = Time.unscaledTime;
        for (int i = 0; i < _sway.Count; i++)
        {
            FoliageSway s = _sway[i];
            if (s.Transform == null)
                continue;
            float a = Mathf.Sin((t * s.Speed) + s.Phase) * s.AmpDeg;
            float b = Mathf.Sin((t * s.Speed * 0.73f) + s.Phase * 1.7f) * (s.AmpDeg * 0.55f);
            s.Transform.localRotation = s.BaseLocal * Quaternion.Euler(a, 0f, b);
        }
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
        foreach (KeyValuePair<string, Texture2D> pair in textures)
        {
            if (string.Equals(pair.Key, name, StringComparison.OrdinalIgnoreCase))
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

    void PushEnv()
    {
        if (!_savedEnv)
        {
            _fogWas = RenderSettings.fog;
            _fogColorWas = RenderSettings.fogColor;
            _fogModeWas = RenderSettings.fogMode;
            _fogStartWas = RenderSettings.fogStartDistance;
            _fogEndWas = RenderSettings.fogEndDistance;
            _fogDensityWas = RenderSettings.fogDensity;
            _ambientWas = RenderSettings.ambientLight;
            _ambientModeWas = RenderSettings.ambientMode;
            _skyWas = RenderSettings.skybox;
            _savedEnv = true;
        }

        RenderSettings.fog = false;
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.38f, 0.46f, 0.48f);
        if (_sun != null)
            _sun.SetActive(true);
    }

    void PopEnv()
    {
        if (!_savedEnv)
            return;
        RenderSettings.fog = _fogWas;
        RenderSettings.fogColor = _fogColorWas;
        RenderSettings.fogMode = _fogModeWas;
        RenderSettings.fogStartDistance = _fogStartWas;
        RenderSettings.fogEndDistance = _fogEndWas;
        RenderSettings.fogDensity = _fogDensityWas;
        RenderSettings.ambientMode = _ambientModeWas;
        RenderSettings.ambientLight = _ambientWas;
        RenderSettings.skybox = _skyWas;
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

    void Restore()
    {
        _live = false;
        _sway.Clear();
        LoginForestMusic.Stop();
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

        PopEnv();
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

    static int BuildMask()
    {
        int mask = 1;
        for (int i = 0; i < ViewLayers.Length; i++)
        {
            int layer = LayerMask.NameToLayer(ViewLayers[i]);
            if (layer >= 0)
                mask |= 1 << layer;
        }

        return mask;
    }

    static Vector3 RemapDrawScale(float ueX, float ueY, float ueZ, Quaternion meshLocal)
    {
        Quaternion inv = Quaternion.Inverse(meshLocal);
        Vector3 s = Vector3.one;
        ApplyAxisScale(ref s, inv * Vector3.forward, ueX);
        ApplyAxisScale(ref s, inv * Vector3.right, ueY);
        ApplyAxisScale(ref s, inv * Vector3.up, ueZ);
        return s;
    }

    static void ApplyAxisScale(ref Vector3 s, Vector3 localAxis, float v)
    {
        Vector3 a = new Vector3(Mathf.Abs(localAxis.x), Mathf.Abs(localAxis.y), Mathf.Abs(localAxis.z));
        if (a.x >= a.y && a.x >= a.z)
            s.x *= v;
        else if (a.y >= a.z)
            s.y *= v;
        else
            s.z *= v;
    }

    static void SetLayer(GameObject go, int layer)
    {
        Transform[] transforms = go.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < transforms.Length; i++)
            transforms[i].gameObject.layer = layer;
    }

    static Shader FindUnlit()
    {
        return Shader.Find("Universal Render Pipeline/Unlit")
            ?? Shader.Find("Unlit/Texture")
            ?? Shader.Find("Sprites/Default");
    }

    static void ApplyTexture(Material mat, Texture2D tex, Vector2 scale)
    {
        mat.mainTexture = tex;
        mat.mainTextureScale = scale;
        if (mat.HasProperty("_BaseMap"))
        {
            mat.SetTexture("_BaseMap", tex);
            mat.SetTextureScale("_BaseMap", scale);
        }

        if (mat.HasProperty("_MainTex"))
        {
            mat.SetTexture("_MainTex", tex);
            mat.SetTextureScale("_MainTex", scale);
        }
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
                break;
            yield return null;
        }

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        Texture2D shot = ScreenCapture.CaptureScreenshotAsTexture();
        if (shot == null)
            yield break;
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
