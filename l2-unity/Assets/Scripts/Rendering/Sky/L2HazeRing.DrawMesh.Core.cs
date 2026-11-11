using System;
using UnityEngine;

public partial class L2HazeRing
{
    void EnsureRing()
    {
        if (_whiteRing == null)
        {
            _whiteRing = Resources.Load<Texture2D>(TexPath);
        }

        Shader shader = Shader.Find(ShaderName);
        if (shader == null || _whiteRing == null)
        {
            return;
        }

        _whiteRing.wrapModeU = TextureWrapMode.Repeat;
        _whiteRing.wrapModeV = TextureWrapMode.Clamp;

        if (_mesh == null || _mesh.name != MeshName)
        {
            if (_mesh != null)
            {
                if (Application.isPlaying)
                    Destroy(_mesh);
                else
                    DestroyImmediate(_mesh);
            }

            _mesh = BuildCylinderMesh();
            _mesh.hideFlags = HideFlags.DontSave;
            if (_mf != null)
            {
                _mf.sharedMesh = _mesh;
            }
        }

        if (_tf != null)
        {
            return;
        }

        var go = new GameObject(MeshObjectName);
        go.hideFlags = HideFlags.HideAndDontSave;
        _tf = go.transform;
        _tf.SetParent(transform, false);
        ApplyHazeLayer(go);

        _mf = go.AddComponent<MeshFilter>();
        _mf.sharedMesh = _mesh;

        _mr = go.AddComponent<MeshRenderer>();
        _mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        _mr.receiveShadows = false;
        _mr.allowOcclusionWhenDynamic = false;
        _mr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        _mr.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

        _mat = new Material(shader);
        _mat.hideFlags = HideFlags.DontSave;
        _mat.SetTexture("_MainTex", _whiteRing);
        Color tint = L2HazeLut.ColorModifierDefault;
        tint.a = Mathf.Clamp01(_opacity);
        ApplyHazeColor(tint);
        _mat.renderQueue = 2980;
        _mr.sharedMaterial = _mat;
    }

    static Mesh BuildCylinderMesh()
    {
        float[] u =
        {
            -0.0058f, 0.1203f, 0.2460f, 0.3705f, 0.4938f, 0.6163f, 0.7388f, 0.8620f,
            0.9866f, 1.1122f, 1.2383f, 1.3640f, 1.4885f, 1.6118f, 1.7343f, 1.8568f,
            1.9801f, 2.1046f, 2.2303f
        };
        float[] unrealX =
        {
            -4.5663f, 4.4634f, 12.9485f, 19.8657f, 24.3805f, 25.9485f, 24.3806f, 19.8657f,
            12.9486f, 4.4634f, -4.5663f, -13.0514f, -19.9686f, -24.4835f, -26.0515f, -24.4835f,
            -19.9686f, -13.0515f, -4.5663f
        };
        float[] unrealY =
        {
            25.6050f, 25.6050f, 22.5167f, 16.7125f, 8.8925f, 0f, -8.8925f, -16.7125f,
            -22.5167f, -25.6050f, -25.6050f, -22.5167f, -16.7125f, -8.8925f, 0f, 8.8925f,
            16.7125f, 22.5167f, 25.6050f
        };
        float[] ringY = { -100f, 0f, 0f, MeshTopY };
        float[] ringV = { 0.9900f, 0.9878f, 0.9900f, 0.0105f };

        const int cols = 19;
        const int rings = 4;
        var positions = new Vector3[cols * rings];
        var uvs = new Vector2[cols * rings];
        for (int r = 0; r < rings; r++)
        {
            for (int i = 0; i < cols; i++)
            {
                int idx = r * cols + i;
                positions[idx] = new Vector3(unrealX[i], ringY[r], unrealY[i]);
                uvs[idx] = new Vector2(u[i], ringV[r]);
            }
        }

        var indices = new int[18 * 2 * 6];
        int t = 0;
        for (int band = 0; band < 2; band++)
        {
            int botRing = band * 2;
            int topRing = botRing + 1;
            for (int i = 0; i < cols - 1; i++)
            {
                int b0 = botRing * cols + i;
                int b1 = botRing * cols + i + 1;
                int t0 = topRing * cols + i;
                int t1 = topRing * cols + i + 1;
                indices[t++] = b0;
                indices[t++] = t0;
                indices[t++] = b1;
                indices[t++] = b1;
                indices[t++] = t0;
                indices[t++] = t1;
            }
        }

        var mesh = new Mesh { name = MeshName };
        mesh.vertices = positions;
        mesh.uv = uvs;
        mesh.triangles = indices;
        mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 400f);
        return mesh;
    }
}
