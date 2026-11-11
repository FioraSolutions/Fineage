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


    sealed class InterludeSfxRunner : MonoBehaviour { }
}
