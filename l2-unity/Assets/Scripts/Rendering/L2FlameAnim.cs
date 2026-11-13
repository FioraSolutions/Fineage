using UnityEngine;

/// <summary>
/// Original Flameset is a texture animation (de_fire_0000 through de_fire_0015).
/// The flame cards share <c>Default_Flame_Final</c>, so one clock drives every torch.
/// </summary>
public sealed class L2FlameAnim : MonoBehaviour
{
    const string MaterialPath = "Data/Textures/FX_E_T/Materials/Default_Flame_Final";
    const string FramePath = "Data/Textures/FX_E_T/de_fire_";
    const int FrameCount = 16;
    const float Step = 0.08f;

    Material _material;
    Texture2D[] _frames;
    int _index;
    float _time;

    void Awake()
    {
        _material = Resources.Load<Material>(MaterialPath);
        _frames = new Texture2D[FrameCount];
        int n = 0;
        for (int i = 0; i < FrameCount; i++)
        {
            Texture2D frame = Resources.Load<Texture2D>(FramePath + i.ToString("0000"));
            if (frame == null)
                continue;
            _frames[n++] = frame;
        }

        if (n == 0)
        {
            _frames = null;
            return;
        }

        if (n != FrameCount)
        {
            var packed = new Texture2D[n];
            for (int i = 0; i < n; i++)
                packed[i] = _frames[i];
            _frames = packed;
        }

        Apply();
    }

    void Update()
    {
        if (_material == null || _frames == null || _frames.Length == 0)
            return;

        _time += Time.deltaTime;
        if (_time < Step)
            return;
        _time = 0f;
        _index = (_index + 1) % _frames.Length;
        Apply();
    }

    void Apply()
    {
        Texture2D frame = _frames[_index];
        _material.SetTexture("_BaseMap", frame);
        if (_material.HasProperty("_MainTex"))
            _material.SetTexture("_MainTex", frame);
    }
}
