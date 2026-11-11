using System;
using UnityEngine;
using UnityEngine.Rendering;

public partial class L2CloudMyst
{
    void PlaceInFrontOfCamera(Camera cam)
    {
        // Original draw is a small strip in the cloudy sky, always in front of
        // the view — not a tube around the player and not at the actor origin.
        Vector3 fwd = cam.transform.forward;
        Vector3 up = Vector3.up;
        Vector3 dir = Vector3.ProjectOnPlane(fwd, up);
        if (dir.sqrMagnitude < 1e-4f)
            dir = Vector3.ProjectOnPlane(cam.transform.up, up);
        if (dir.sqrMagnitude < 1e-4f)
            dir = Vector3.forward;
        dir.Normalize();
        dir = (dir + up * _viewLift).normalized;
        float dist = Mathf.Max(8f, _viewDistance);
        _tf.position = cam.transform.position + dir * dist;
        _tf.rotation = Quaternion.LookRotation(dir, up);
    }

    void EnsureMesh()
    {
        if (_color == null)
            _color = Resources.Load<Texture2D>(TexColor);
        if (_alpha == null)
            _alpha = Resources.Load<Texture2D>(TexAlpha);
        if (_noise == null)
            _noise = Resources.Load<Texture2D>(TexNoise);

        Shader shader = Shader.Find(ShaderName);
        if (shader == null || _color == null || _alpha == null || _noise == null)
            return;

        _color.wrapModeU = TextureWrapMode.Repeat;
        _color.wrapModeV = TextureWrapMode.Clamp;
        _alpha.wrapModeU = TextureWrapMode.Repeat;
        _alpha.wrapModeV = TextureWrapMode.Clamp;
        _noise.wrapModeU = TextureWrapMode.Repeat;
        _noise.wrapModeV = TextureWrapMode.Repeat;

        if (_mesh == null || _mesh.name != L2CloudMeshes.CylName)
        {
            DestroyObj(_mesh);
            _mesh = L2CloudMeshes.BuildCyl();
            _mesh.hideFlags = HideFlags.DontSave;
            if (_mf != null)
                _mf.sharedMesh = _mesh;
        }

        if (_tf != null)
            return;

        var go = new GameObject(MeshObjectName);
        go.hideFlags = HideFlags.HideAndDontSave;
        go.layer = 0;
        _tf = go.transform;
        _tf.SetParent(transform, false);

        _mf = go.AddComponent<MeshFilter>();
        _mf.sharedMesh = _mesh;
        _mr = go.AddComponent<MeshRenderer>();
        _mr.shadowCastingMode = ShadowCastingMode.Off;
        _mr.receiveShadows = false;
        _mr.allowOcclusionWhenDynamic = false;
        _mr.lightProbeUsage = LightProbeUsage.Off;
        _mr.reflectionProbeUsage = ReflectionProbeUsage.Off;

        _mat = new Material(shader);
        _mat.hideFlags = HideFlags.DontSave;
        _mat.SetTexture("_MainTex", _color);
        _mat.SetTexture("_AlphaTex", _alpha);
        _mat.SetTexture("_NoiseTex", _noise);
        _mat.SetVector(CloudColorId, Vector4.one);
        _mat.SetVector("_NoiseTiling", new Vector4(5f, 2.5f, 0f, 0f));
        _mat.SetVector("_NoisePan", new Vector4(0.05f, 0.02f, 0f, 0f));
        _mat.SetVector("_SkyParams", new Vector4(88f, 0f, 0f, 0f));
        _mat.renderQueue = 2978;
        _mr.sharedMaterial = _mat;
    }

    float ResolveSkyDistance(Camera cam)
    {
        float far = cam != null ? cam.farClipPlane : 500f;
        return Mathf.Clamp(far * 0.88f, 80f, 4000f);
    }

    Camera ResolveCamera()
    {
        if (CameraPacketServices.Controller != null)
        {
            Camera fromController = CameraPacketServices.Controller.GetComponent<Camera>();
            if (fromController == null)
                fromController = CameraPacketServices.Controller.GetComponentInChildren<Camera>();
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

    void DestroyMesh()
    {
        DestroyObj(_mat);
        _mat = null;
        DestroyObj(_mesh);
        _mesh = null;
        if (_tf != null)
            DestroyObj(_tf.gameObject);
        _tf = null;
        _mf = null;
        _mr = null;
    }

    static void DestroyObj(UnityEngine.Object obj)
    {
        if (obj == null)
            return;
        if (Application.isPlaying)
            Destroy(obj);
        else
            DestroyImmediate(obj);
    }
}
