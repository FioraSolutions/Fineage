using System;
using UnityEngine;

public partial class L2StarDome : MonoBehaviour
{
    static void ConfigureTex(Texture2D tex)
    {
        tex.wrapModeU = TextureWrapMode.Repeat;
        tex.wrapModeV = TextureWrapMode.Repeat;
    }

    static Material MakeMat(Shader shader, Texture2D tex, Color tint, int queue)
    {
        var mat = new Material(shader);
        mat.hideFlags = HideFlags.DontSave;
        mat.SetTexture("_MainTex", tex);
        mat.SetVector(StarColorId, new Vector4(tint.r, tint.g, tint.b, tint.a));
        mat.renderQueue = queue;
        return mat;
    }

    float ResolveSkyDistance(Camera cam)
    {
        float far = cam != null ? cam.farClipPlane : 500f;
        return Mathf.Clamp(far * 0.88f, 80f, 4000f);
    }

    void ApplySkyLayer(GameObject go)
    {
        if (L2FxCompositorRuntime.PreferGpuQueue)
            L2FxCompositorLayers.ApplyL2SkyLayer(go);
        else
            go.layer = 0;
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

    void DestroyDome()
    {
        DestroyMat(ref _smallMat);
        DestroyMat(ref _largeMat);

        if (_mesh != null)
        {
            if (Application.isPlaying)
                Destroy(_mesh);
            else
                DestroyImmediate(_mesh);
            _mesh = null;
        }

        if (_tf != null)
        {
            if (Application.isPlaying)
                Destroy(_tf.gameObject);
            else
                DestroyImmediate(_tf.gameObject);
            _tf = null;
        }

        _mf = null;
        _mr = null;
    }

    static void DestroyMat(ref Material mat)
    {
        if (mat == null)
        {
            return;
        }

        if (Application.isPlaying)
            Destroy(mat);
        else
            DestroyImmediate(mat);
        mat = null;
    }
}
