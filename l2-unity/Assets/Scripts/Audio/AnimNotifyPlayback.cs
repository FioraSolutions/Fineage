using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Plays official ukx animation notifies on the bound Animator.
/// One shared path for footsteps, swings, breath, and other AnimNotify_Sound /
/// SwimSound / AttackVoice bindings. Times come from the dump table only.
/// </summary>
public sealed class AnimNotifyPlayback : MonoBehaviour
{
    [SerializeField] Animator _animator;
    [SerializeField] SurfaceDetector _surfaceDetector;
    [SerializeField] string _animationHint;

    string _clipName;
    string _sequence;
    string _sequenceError;
    float _lastNormalized;
    bool _hasLast;
    readonly HashSet<string> _logged = new HashSet<string>();
    readonly HashSet<string> _firedThisLoop = new HashSet<string>();
    public string AnimationHint
    {
        get => _animationHint;
        set => _animationHint = value;
    }

    public string BoundSequence => _sequence;

    void Awake()
    {
        if (_animator == null)
            _animator = GetComponent<Animator>();
        if (_animator == null)
            _animator = GetComponentInChildren<Animator>();
        if (_surfaceDetector == null)
            _surfaceDetector = GetComponent<SurfaceDetector>();
    }

    void LateUpdate()
    {
        if (_animator == null || !_animator.enabled)
            return;

        AnimNotifyTable.Load();
        AnimatorClipInfo[] infos = _animator.GetCurrentAnimatorClipInfo(0);
        if (infos == null || infos.Length == 0 || infos[0].clip == null)
            return;

        string clipName = infos[0].clip.name;
        AnimatorStateInfo state = _animator.GetCurrentAnimatorStateInfo(0);
        float normalized = state.normalizedTime;
        float cycle = normalized - Mathf.Floor(normalized);

        if (!string.Equals(clipName, _clipName, System.StringComparison.Ordinal))
        {
            _clipName = clipName;
            _hasLast = false;
            _firedThisLoop.Clear();
            BindSequence(clipName);
        }

        if (string.IsNullOrEmpty(_sequence))
            return;

        if (!_hasLast)
        {
            _lastNormalized = cycle;
            _hasLast = true;
            // Entering mid-cycle must not fire already-passed notifies.
            // Time-0 (or very early) notifies still fire when the clip starts at the beginning.
            if (cycle <= 0.0001f)
                FireRange(-0.0001f, cycle, wrap: false);
            return;
        }

        if (cycle + 0.0001f < _lastNormalized)
        {
            FireRange(_lastNormalized, 1f, wrap: false);
            _firedThisLoop.Clear();
            FireRange(0f, cycle, wrap: false);
        }
        else
        {
            FireRange(_lastNormalized, cycle, wrap: false);
        }

        _lastNormalized = cycle;
    }

    void BindSequence(string clipName)
    {
        _sequence = null;
        _sequenceError = null;
        if (AnimNotifySequenceResolver.TryResolve(clipName, _animationHint, out string sequence, out string error))
        {
            _sequence = sequence;
            return;
        }

        _sequenceError = error;
        if (!string.IsNullOrEmpty(error) && _logged.Add("seq:" + clipName))
            Log(error);
    }

    void FireRange(float fromExclusive, float toInclusive, bool wrap)
    {
        if (!AnimNotifyTable.TryGetByMeshSequence(_animationHint, _sequence, out var bindings)
            && !AnimNotifyTable.TryGetBySequence(_sequence, out bindings))
        {
            if (_logged.Add("empty:" + _sequence))
                Log("AnimNotifyPlayback: sequence=\"" + _sequence + "\" has no notify bindings clip=\"" + _clipName + "\".");
            return;
        }

        for (int i = 0; i < bindings.Count; i++)
        {
            AnimNotifyBinding n = bindings[i];
            if (n.Time < 0f || n.Time > 1.0001f)
            {
                if (_logged.Add("time:" + n.Id))
                    Log("AnimNotifyPlayback: notify time=" + n.Time.ToString("G9")
                        + " is outside 0..1, skipped id=" + n.Id);
                continue;
            }

            bool inRange = n.Time > fromExclusive + 0.00001f && n.Time <= toInclusive + 0.00001f;
            if (!inRange)
                continue;
            if (!_firedThisLoop.Add(n.Id))
                continue;
            Play(n);
        }
    }

    void Play(AnimNotifyBinding binding)
    {
        if (string.IsNullOrEmpty(binding.ClassName))
        {
            if (_logged.Add("class:" + binding.Id))
                Log("AnimNotifyPlayback: notify has no class, skipped id=" + binding.Id);
            return;
        }

        if (binding.ClassName == "AnimNotify_Sound"
            || binding.ClassName == "AnimNotify_SwimSound"
            || binding.ClassName == "AnimNotify_AttackVoice")
        {
            PlaySound(binding);
            return;
        }

        // AttackShot / AttackPreShot / AttackItem / Channeling / Effect / ViewShake
        // stay on the existing clip-event / combat path so those systems are not
        // double-fired. Their official times remain in the table.
    }

    void PlaySound(AnimNotifyBinding binding)
    {
        bool running = AnimNotifySoundResolver.IsRunSequence(binding.Sequence);
        string tag = _surfaceDetector != null ? _surfaceDetector.GetSurfaceTag() : null;
        string wave = AnimNotifySoundResolver.ResolveWave(binding, tag, running, out string resolveLog);
        if (!string.IsNullOrEmpty(resolveLog) && _logged.Add("surf:" + resolveLog))
            Log(resolveLog);
        if (string.IsNullOrEmpty(wave))
        {
            if (!string.IsNullOrEmpty(resolveLog))
                return;
            Log(AnimNotifySoundResolver.FormatMissingSoundError(binding.Id));
            return;
        }

        AudioService service = RequiredAudioServices.RequireService(nameof(AnimNotifyPlayback));
        if (service == null)
            return;

        float volume = AnimNotifySoundResolver.VolumeScale(binding.Volume);
        if (!AnimNotifySoundResolver.TryPlayWave(service, wave, transform.position, volume, out string playError))
        {
            if (_logged.Add("wave:" + wave))
                Log(playError ?? AnimNotifySoundResolver.FormatMissingWaveError(wave, binding.Id));
        }
    }

    void Log(string message)
    {
        AnimNotifyTable.ErrorLogged?.Invoke(message);
        Debug.LogError(message);
    }
}
