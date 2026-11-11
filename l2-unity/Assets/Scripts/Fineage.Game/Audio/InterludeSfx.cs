using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class InterludeSfx
{
    const string HostName = "FineageInterludeSfx2D";
    static readonly Dictionary<string, AudioClip> Cache = new Dictionary<string, AudioClip>();
    static readonly Dictionary<string, float> MissUntil = new Dictionary<string, float>();
    const float MissRetrySeconds = 2f;

    static AudioSource _oneShotSrc;
    static AudioSource _castSrc;
    static AudioSource _voiceSrc;
    static InterludeSfxRunner _runner;


    public static bool IsCastPlaying =>
        _castSrc != null && _castSrc.isPlaying;

    public static float CastRemainingSec
    {
        get
        {
            if (_castSrc == null || !_castSrc.isPlaying || _castSrc.clip == null)
                return 0f;
            return Mathf.Max(0f, _castSrc.clip.length - _castSrc.time);
        }
    }

    sealed class InterludeSfxRunner : MonoBehaviour { }
}
