using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Applies the Lobby.unr login vista through the shared actor transform:
/// the camera named by LoginVista.json, ZoneInfo distance fog, the level
/// ambient, and NMovableSunLight. The baked tile is the scene. This does not
/// spawn a side forest or hide the terrain sheet.
/// </summary>
public static class L2LoginVista
{
    const string EnvMap = "Lobby";

    static bool _saved;
    static bool _savedFog;
    static FogMode _savedFogMode;
    static Color _savedFogColor;
    static float _savedFogStart;
    static float _savedFogEnd;
    static AmbientMode _savedAmbientMode;
    static Color _savedAmbient;
    static Light _sun;
    static Quaternion _savedSunRotation;
    static Color _savedSunColor;
    static float _savedSunIntensity;
    static bool _createdSun;

    public static void Apply(Camera cam)
    {
        if (cam == null)
        {
            Debug.LogError("[LoginVista] no login camera");
            return;
        }

        TextAsset json = Resources.Load<TextAsset>(L2LoginVistaPose.ResourcePath);
        if (json == null)
        {
            Debug.LogError("[LoginVista] missing Resources/" + L2LoginVistaPose.ResourcePath
                + ".json (tools/interlude-extract/build_login_vista.py)");
            return;
        }

        L2LoginVistaFile file = L2LoginVistaPose.Parse(json.text);
        if (file == null)
            return;

        if (!file.actionsObjectArray)
        {
            Debug.LogError("[LoginVista] SceneManager0 Tag=Logon_Warp Actions has no object_array in the property dump; camera is "
                + file.cameraActor + ". " + file.cameraReason);
        }

        L2Ue2ActorXform.Pose pose = L2LoginVistaPose.CameraPose(file);
        cam.transform.SetPositionAndRotation(pose.position, pose.rotation);
        float aspect = cam.aspect;
        if (aspect <= 0.01f)
        {
            Debug.LogError("[LoginVista] camera aspect " + aspect + " is not positive; vertical FOV uses 16:9");
            aspect = 16f / 9f;
        }

        cam.fieldOfView = L2LoginVistaPose.VerticalFov(file, aspect);
        float fogEnd = L2LoginVistaPose.FogEndMeters(file);
        if (cam.farClipPlane < fogEnd)
            cam.farClipPlane = fogEnd;

        Remember();
        ApplyFog(file);
        ApplyAmbient(file);
        ApplySun(file);
        ApplyEmitters();
        WarnIfUnderSheet(cam);
        WarnIfArchMissing();
    }

    public static void Restore()
    {
        OfficialWorldEmitterPlayback.ActiveEnvMap = null;
        if (!_saved)
            return;

        RenderSettings.fog = _savedFog;
        RenderSettings.fogMode = _savedFogMode;
        RenderSettings.fogColor = _savedFogColor;
        RenderSettings.fogStartDistance = _savedFogStart;
        RenderSettings.fogEndDistance = _savedFogEnd;
        RenderSettings.ambientMode = _savedAmbientMode;
        RenderSettings.ambientLight = _savedAmbient;
        if (_createdSun && _sun != null)
            Object.Destroy(_sun.gameObject);
        else if (_sun != null)
        {
            _sun.transform.rotation = _savedSunRotation;
            _sun.color = _savedSunColor;
            _sun.intensity = _savedSunIntensity;
        }

        _sun = null;
        _createdSun = false;
        _saved = false;
    }

    static void Remember()
    {
        if (_saved)
            return;
        _savedFog = RenderSettings.fog;
        _savedFogMode = RenderSettings.fogMode;
        _savedFogColor = RenderSettings.fogColor;
        _savedFogStart = RenderSettings.fogStartDistance;
        _savedFogEnd = RenderSettings.fogEndDistance;
        _savedAmbientMode = RenderSettings.ambientMode;
        _savedAmbient = RenderSettings.ambientLight;
        _saved = true;
    }

    static void ApplyFog(L2LoginVistaFile file)
    {
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogColor = L2LoginVistaPose.FogColor(file);
        RenderSettings.fogStartDistance = L2LoginVistaPose.FogStartMeters(file);
        RenderSettings.fogEndDistance = L2LoginVistaPose.FogEndMeters(file);
        if (RenderSettings.skybox == null)
        {
            Debug.LogError("[LoginVista] no skybox material; camera clear is " + file.zoneActor
                + " distance-fog color. The lobby sky mesh was not in the scene.");
        }
    }

    static void ApplyAmbient(L2LoginVistaFile file)
    {
        if (file.ambient == null)
        {
            Debug.LogError("[LoginVista] LevelInfo ambient is missing from LoginVista.json");
            return;
        }

        float b = file.ambient.brightness == 0f ? 1f : file.ambient.brightness;
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(file.ambient.r, file.ambient.g, file.ambient.b, 1f) * b;
    }

    static void ApplySun(L2LoginVistaFile file)
    {
        if (file.sun == null)
        {
            Debug.LogError("[LoginVista] NMovableSunLight is missing from LoginVista.json");
            return;
        }

        Light sun = null;
        Light[] lights = Object.FindObjectsByType<Light>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        for (int i = 0; i < lights.Length; i++)
        {
            if (lights[i] != null && lights[i].type == LightType.Directional)
            {
                sun = lights[i];
                break;
            }
        }

        if (sun == null)
        {
            Debug.LogError("[LoginVista] no directional light in the login scene; creating one from "
                + file.sun.name);
            var go = new GameObject(file.sun.name);
            sun = go.AddComponent<Light>();
            sun.type = LightType.Directional;
            _createdSun = true;
        }

        _sun = sun;
        _savedSunRotation = sun.transform.rotation;
        _savedSunColor = sun.color;
        _savedSunIntensity = sun.intensity;
        Vector3 euler = VectorUtils.ConvertRotToUnity(new Vector3(file.sun.pitch, file.sun.yaw, file.sun.roll));
        sun.transform.rotation = Quaternion.Euler(euler);
        sun.color = L2StaticMeshVertexLight.LightPlane(file.sun.hue, file.sun.saturation, 255f);
        // Engine.u/Light.uc defaultproperties LightBrightness=64 is Unity intensity 1.
        sun.intensity = file.sun.brightness / 64f;
    }

    static void ApplyEmitters()
    {
        OfficialWorldEmitterPlayback.ActiveEnvMap = EnvMap;
        if (Object.FindAnyObjectByType<OfficialWorldEmitterPlayback>() == null)
        {
            var host = new GameObject("OfficialWorldEmittersHost");
            host.AddComponent<OfficialWorldEmitterPlayback>();
        }
    }

    static void WarnIfUnderSheet(Camera cam)
    {
        Terrain terrain = Terrain.activeTerrain;
        if (terrain == null)
        {
            Debug.LogError("[LoginVista] no Terrain in the login scene; TerrainInfo0 was not baked");
            return;
        }

        float sheet = terrain.SampleHeight(cam.transform.position) + terrain.transform.position.y;
        float under = sheet - cam.transform.position.y;
        if (under > 0.05f)
        {
            Debug.LogError("[LoginVista] " + cam.name + " is " + (under / VectorUtils.L2UuToUnity).ToString("F1")
                + " UU below the terrain sheet at its XY. The sheet is still drawn.");
        }
    }

    static void WarnIfArchMissing()
    {
        Renderer[] rends = Object.FindObjectsByType<Renderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < rends.Length; i++)
        {
            if (rends[i] != null && rends[i].name.IndexOf("lobby_back", System.StringComparison.OrdinalIgnoreCase) >= 0)
                return;
        }

        Debug.LogError("[LoginVista] baked scene has no lobby_back (StaticMeshActor73). "
            + "Bake l2_lobby through L2InterludeTileBakePipeline; the login does not build a side scene.");
    }
}
