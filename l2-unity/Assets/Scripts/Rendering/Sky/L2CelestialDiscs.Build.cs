using System;
using Object = UnityEngine.Object;

public partial class L2CelestialDiscs
{
    void EnsureDiscs()
    {
        if (_sunTexture == null)
        {
            _sunTexture = Resources.Load<Texture2D>(SunTexPath);
        }

        if (_moonTexture == null)
        {
            _moonTexture = Resources.Load<Texture2D>(MoonTexPath);
        }

        Shader shader = Shader.Find(ShaderName);
        if (shader == null)
        {
            return;
        }

        if (_sunTf == null)
        {
            CreateDisc("L2SunDisc", _sunTexture, shader, out _sunTf, out _sunMr, out _sunMat);
        }

        if (_moonTf == null)
        {
            CreateDisc("L2MoonDisc", _moonTexture, shader, out _moonTf, out _moonMr, out _moonMat);
        }
    }

    void CreateDisc(string name, Texture2D tex, Shader shader, out Transform tf, out MeshRenderer mr, out Material mat)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
        go.name = name;
        go.hideFlags = HideFlags.HideAndDontSave;
        Object.DestroyImmediate(go.GetComponent<Collider>());

        tf = go.transform;
        tf.SetParent(null, true);
        ApplyCelestialLayer(go);

        mr = go.GetComponent<MeshRenderer>();
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mr.receiveShadows = false;
        mr.allowOcclusionWhenDynamic = false;
        mr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        mr.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

        mat = new Material(shader);
        mat.hideFlags = HideFlags.DontSave;
        mat.SetTexture("_MainTex", tex);
        mat.SetColor("_Color", Color.white);
        mr.sharedMaterial = mat;
    }

    void ApplyCelestialLayer(GameObject go)
    {
        if (L2FxCompositorRuntime.PreferGpuQueue)
            L2FxCompositorLayers.ApplyL2SkyLayer(go);
        else
            go.layer = 0;
    }

    public void DrawSunBloomExclude(CommandBuffer cmd)
    {
        if (cmd == null || _sunMr == null || !_sunMr.enabled || _sunMat == null)
            return;

        int pass = _sunMat.FindPass("BloomExclude");
        if (pass < 0)
            return;

        cmd.DrawRenderer(_sunMr, _sunMat, 0, pass);
    }

    public void DrawMoonBloomInclude(CommandBuffer cmd)
    {
        if (cmd == null || _moonMr == null || !_moonMr.enabled || _moonMat == null)
            return;

        int pass = _moonMat.FindPass("BloomInclude");
        if (pass < 0)
            return;

        cmd.DrawRenderer(_moonMr, _moonMat, 0, pass);
    }

    void LateUpdateApplyLayer()
    {
        if (_sunTf != null)
            ApplyCelestialLayer(_sunTf.gameObject);
        if (_moonTf != null)
            ApplyCelestialLayer(_moonTf.gameObject);
    }

    Camera ResolveCamera()
    {
        if (CameraPacketServices.Controller != null)
        {
            Camera fromController = CameraPacketServices.Controller.GetComponent<Camera>();
            if (fromController == null)
            {
                fromController = CameraPacketServices.Controller.GetComponentInChildren<Camera>();
            }

            if (fromController != null && fromController.enabled && fromController.gameObject.activeInHierarchy)
            {
                _cam = fromController;
                return _cam;
            }
        }

        Camera main = Camera.main;
        if (main != null && main.enabled && main.gameObject.activeInHierarchy)
        {
            _cam = main;
            return _cam;
        }

        Camera[] cams = Camera.allCameras;
        for (int i = 0; i < cams.Length; i++)
        {
            Camera c = cams[i];
            if (c != null && c.enabled && c.gameObject.activeInHierarchy && !c.orthographic)
            {
                _cam = c;
                return _cam;
            }
        }

        return _cam;
    }

    static void DestroyDisc(ref Transform tf, ref Material mat)
    {
        if (mat != null)
        {
            if (Application.isPlaying)
            {
                Destroy(mat);
            }
            else
            {
                DestroyImmediate(mat);
            }

            mat = null;
        }

        if (tf != null)
        {
            if (Application.isPlaying)
            {
                Destroy(tf.gameObject);
            }
            else
            {
                DestroyImmediate(tf.gameObject);
            }

            tf = null;
        }
    }
}
