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
    Renderer[] _renderers;
    readonly HashSet<string> _logged = new HashSet<string>();
    readonly HashSet<string> _firedThisLoop = new HashSet<string>();
    static Plane[] _frustum = new Plane[6];
    static int _frustumFrame = -1;
    static Camera _frameCamera;
    static bool _loggedNoCamera;
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
        CacheRenderers();
    }

    void OnEnable()
    {
        CacheRenderers();
    }

    void CacheRenderers()
    {
        _renderers = GetComponentsInChildren<Renderer>(true);
    }

    void LateUpdate()
    {
        if (_animator == null || !_animator.enabled)
            return;

        // Animator.cullingMode does not stop this MonoBehaviour. AlwaysAnimate and
        // CullUpdateTransforms keep advancing off-screen, so LateUpdate would otherwise
        // fire every notify in the world. Audio follows the camera frustum, not the cull mode.
        if (!IsRenderedForAudio())
        {
            _hasLast = false;
            return;
        }

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

        if (!AnimNotifyTable.TryGetByMeshSequence(_animationHint, _sequence, out var bindings)
            && !AnimNotifyTable.TryGetBySequence(_sequence, out bindings))
        {
            if (_logged.Add("empty:" + _sequence))
                Log("AnimNotifyPlayback: sequence=\"" + _sequence + "\" has no notify bindings clip=\"" + _clipName + "\".");
            return;
        }

        float rate = bindings.Count > 0 ? bindings[0].Rate : 0f;
        int frames = bindings.Count > 0 ? bindings[0].FrameCount : 0;
        AnimNotifyPlaybackWindow.Result step = AnimNotifyPlaybackWindow.Step(
            _hasLast, _lastNormalized, cycle, Time.deltaTime, rate, frames);
        _lastNormalized = step.Cursor;
        _hasLast = true;
        if (!string.IsNullOrEmpty(step.Error) && _logged.Add("rate:" + _sequence))
            Log(step.Error + " sequence=\"" + _sequence + "\" clip=\"" + _clipName + "\".");
        // A blend that restarts the same clip looks like a loop wrap. The new cycle
        // fires its own notifies after the transition; emitting the wrap would double them.
        if (step.Wrapped && _animator.IsInTransition(0))
        {
            _firedThisLoop.Clear();
            return;
        }

        if (!step.Emit)
            return;

        bool latestOnly = AnimNotifyPlaybackWindow.ShouldPlayOnlyLatest(step.Span, step.OneFrame);
        AnimNotifyBinding chosen = null;
        float chosenKey = float.NegativeInfinity;
        if (step.Wrapped)
        {
            FireRange(bindings, step.FromExclusive, 1f, latestOnly, 0f, ref chosen, ref chosenKey);
            _firedThisLoop.Clear();
            FireRange(bindings, 0f, step.ToInclusive, latestOnly, 1f, ref chosen, ref chosenKey);
        }
        else
        {
            FireRange(bindings, step.FromExclusive, step.ToInclusive, latestOnly, 0f, ref chosen, ref chosenKey);
        }

        if (latestOnly && chosen != null)
            Play(chosen);
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

    void FireRange(
        System.Collections.Generic.IReadOnlyList<AnimNotifyBinding> bindings,
        float fromExclusive,
        float toInclusive,
        bool latestOnly,
        float keyBias,
        ref AnimNotifyBinding chosen,
        ref float chosenKey)
    {
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
            if (!latestOnly)
            {
                Play(n);
                continue;
            }

            if (!IsSoundNotify(n))
                continue;
            float key = n.Time + keyBias;
            if (key < chosenKey)
                continue;
            chosenKey = key;
            chosen = n;
        }
    }

    static bool IsSoundNotify(AnimNotifyBinding binding) =>
        binding.ClassName == "AnimNotify_Sound"
        || binding.ClassName == "AnimNotify_SwimSound"
        || binding.ClassName == "AnimNotify_AttackVoice";

    bool IsRenderedForAudio()
    {
        if (_renderers == null)
            CacheRenderers();
        if (_renderers == null || _renderers.Length == 0)
        {
            if (_logged.Add("norenderer"))
                Log("AnimNotifyPlayback: no Renderer on \"" + name + "\", anim notifies will not play audio.");
            return false;
        }

        bool anyVisible = false;
        for (int i = 0; i < _renderers.Length; i++)
        {
            Renderer renderer = _renderers[i];
            if (renderer != null && renderer.enabled && renderer.isVisible)
            {
                anyVisible = true;
                break;
            }
        }

        if (!anyVisible)
            return false;

        if (_frustumFrame != Time.frameCount)
        {
            _frustumFrame = Time.frameCount;
            _frameCamera = Camera.main;
            if (_frameCamera != null)
                GeometryUtility.CalculateFrustumPlanes(_frameCamera, _frustum);
        }

        if (_frameCamera == null)
        {
            if (!_loggedNoCamera)
            {
                _loggedNoCamera = true;
                Log("AnimNotifyPlayback: no Camera.main, anim-notify audio skipped.");
            }
            return false;
        }

        for (int i = 0; i < _renderers.Length; i++)
        {
            Renderer renderer = _renderers[i];
            if (renderer == null || !renderer.enabled || !renderer.isVisible)
                continue;
            if (GeometryUtility.TestPlanesAABB(_frustum, renderer.bounds))
                return true;
        }

        return false;
    }

    void Play(AnimNotifyBinding binding)
    {
        if (string.IsNullOrEmpty(binding.ClassName))
        {
            if (_logged.Add("class:" + binding.Id))
                Log("AnimNotifyPlayback: notify has no class, skipped id=" + binding.Id);
            return;
        }

        if (IsSoundNotify(binding))
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
            if (_logged.Add("nosound:" + binding.Id))
                Log(AnimNotifySoundResolver.FormatMissingSoundError(binding.Id));
            return;
        }

        if (!AnimNotifySoundResolver.TryVolumeScale(binding.Volume, out float volume))
        {
            if (_logged.Add("vol:" + binding.Id))
                Log("AnimNotifyPlayback: missing Volume notify=" + binding.Id
                    + " sound=\"" + binding.Sound + "\".");
            return;
        }

        AudioService service = RequiredAudioServices.RequireService(nameof(AnimNotifyPlayback));
        if (service == null)
            return;

        string contextHint = binding.Package + " " + binding.Animation + " " + binding.Sequence;
        string id = contextHint + " wave=\"" + wave + "\" notify=" + binding.Id;
        if (!AnimNotifySoundResolver.TryPlayWave(
            service, wave, transform.position, volume, binding.Radius, out string playError, contextHint,
            GetInstanceID(), Ue2SoundAudibility.SlotForNotifyClass(binding.ClassName), id,
            "AnimNotify", name))
        {
            if (_logged.Add("wave:" + wave))
                Log(playError ?? AnimNotifySoundResolver.FormatMissingWaveError(wave, binding.Id));
        }
    }

    void Log(string message)
    {
        if (AnimNotifyTable.ErrorLogged != null)
            AnimNotifyTable.ErrorLogged.Invoke(message);
        if (!AudioErrorThrottle.TryEmit("anim:" + message, Time.unscaledTime, once: true, out _))
            return;
        Debug.LogError(message);
    }
}
