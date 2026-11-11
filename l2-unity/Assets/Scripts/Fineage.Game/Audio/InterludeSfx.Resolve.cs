using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class InterludeSfx
{
    public static void PlayAfter(float delaySeconds, System.Action action)
    {
        if (action == null)
            return;
        if (delaySeconds <= 0f)
        {
            action();
            return;
        }
        InterludeSfxRunner runner = EnsureRunner();
        if (runner == null)
        {
            action();
            return;
        }
        runner.StartCoroutine(DelayThen(delaySeconds, action));
    }

    static IEnumerator DelayThen(float delay, System.Action action)
    {
        yield return new WaitForSecondsRealtime(delay);
        action();
    }

    static AudioSource EnsureOneShotSource()
    {
        EnsureRunner();
        if (_oneShotSrc != null)
            return _oneShotSrc;
        _oneShotSrc = CreateChildSource("OneShot");
        return _oneShotSrc;
    }

    static AudioSource EnsureCastSource()
    {
        EnsureRunner();
        if (_castSrc != null)
            return _castSrc;
        _castSrc = CreateChildSource("Cast");
        return _castSrc;
    }

    static AudioSource EnsureVoiceSource()
    {
        EnsureRunner();
        if (_voiceSrc != null)
            return _voiceSrc;
        _voiceSrc = CreateChildSource("Voice");
        return _voiceSrc;
    }

    static AudioSource CreateChildSource(string childName)
    {
        if (_runner == null)
            return null;
        Transform existing = _runner.transform.Find(childName);
        GameObject go = existing != null ? existing.gameObject : new GameObject(childName);
        if (existing == null)
            go.transform.SetParent(_runner.transform, false);
        AudioSource src = go.GetComponent<AudioSource>();
        if (src == null)
            src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.spatialBlend = 0f;
        src.loop = false;
        src.volume = 1f;
        return src;
    }

    static InterludeSfxRunner EnsureRunner()
    {
        if (_runner != null)
            return _runner;
        GameObject go = GameObject.Find(HostName);
        if (go == null)
        {
            go = new GameObject(HostName);
            Object.DontDestroyOnLoad(go);
        }
        _runner = go.GetComponent<InterludeSfxRunner>();
        if (_runner == null)
            _runner = go.AddComponent<InterludeSfxRunner>();
        return _runner;
    }
}
