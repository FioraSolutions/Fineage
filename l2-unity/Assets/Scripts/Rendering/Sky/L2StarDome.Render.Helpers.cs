using System;
using UnityEngine;

public partial class L2StarDome : MonoBehaviour
{
    static void ApplyTint(Material mat, Color nightTint, float fade)
    {
        if (mat == null)
        {
            return;
        }

        mat.SetVector(StarColorId, new Vector4(
            nightTint.r * fade,
            nightTint.g * fade,
            nightTint.b * fade,
            nightTint.a));
    }

    void EnsureDome()
    {
        if (_smallStars == null)
        {
            _smallStars = Resources.Load<Texture2D>(SmallTexPath);
        }

        if (_largeStars == null)
        {
            _largeStars = Resources.Load<Texture2D>(LargeTexPath);
        }

        Shader shader = Shader.Find(ShaderName);
        if (shader == null || _smallStars == null || _largeStars == null)
        {
            return;
        }

        ConfigureTex(_smallStars);
        ConfigureTex(_largeStars);

        if (_mesh == null || _mesh.name != L2StarDomeMesh.MeshName)
        {
            if (_mesh != null)
            {
                if (Application.isPlaying)
                    Destroy(_mesh);
                else
                    DestroyImmediate(_mesh);
            }

            _mesh = L2StarDomeMesh.Build();
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
        ApplySkyLayer(go);

        _mf = go.AddComponent<MeshFilter>();
        _mf.sharedMesh = _mesh;

        _mr = go.AddComponent<MeshRenderer>();
        _mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        _mr.receiveShadows = false;
        _mr.allowOcclusionWhenDynamic = false;
        _mr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        _mr.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

        _smallMat = MakeMat(shader, _smallStars, SmallNightTint, 2990);
        _largeMat = MakeMat(shader, _largeStars, LargeNightTint, 2991);
        _mr.sharedMaterials = new[] { _smallMat, _largeMat };
    }
}
