using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Retail Interlude startup splash. Frames are systextures/sp_32b_01..11
/// (Lineage2Ver121), flattened from the client extract. Music is music/intro.ogg.
/// Click, a key, or the end of the cycle dismisses it. Once per process.
/// </summary>
public sealed class InterludeSplashPlayer : MonoBehaviour
{
    public const string ShotPathFile = "/tmp/fineage-interlude-splash.outpath";

    const float FrameSeconds = 2.25f;
    const float SkipLockSeconds = 1.5f;

    static readonly string[] Frames =
    {
        "Data/UI/Splash/Interlude/sp_32b_01",
        "Data/UI/Splash/Interlude/sp_32b_02",
        "Data/UI/Splash/Interlude/sp_32b_03",
        "Data/UI/Splash/Interlude/sp_32b_04",
        "Data/UI/Splash/Interlude/sp_32b_05",
        "Data/UI/Splash/Interlude/sp_32b_06",
        "Data/UI/Splash/Interlude/sp_32b_07",
        "Data/UI/Splash/Interlude/sp_32b_08",
        "Data/UI/Splash/Interlude/sp_32b_09",
        "Data/UI/Splash/Interlude/sp_32b_10",
        "Data/UI/Splash/Interlude/sp_32b_11",
    };

    static bool _played;

    VisualElement _overlay;
    VisualElement _frame;
    Texture2D[] _textures;
    AudioSource _music;
    bool _dismissed;
    float _shownAt;

    public static void Present(VisualElement documentRoot)
    {
        if (_played || documentRoot == null)
            return;
        if (documentRoot.Q("InterludeSplash") != null)
            return;

        var textures = new Texture2D[Frames.Length];
        int count = 0;
        for (int i = 0; i < Frames.Length; i++)
        {
            textures[i] = Resources.Load<Texture2D>(Frames[i]);
            if (textures[i] != null)
                count++;
        }

        if (count == 0)
        {
            Debug.LogWarning("[InterludeSplash] no frames under Resources/Data/UI/Splash/Interlude");
            return;
        }

        _played = true;
        var host = new GameObject("InterludeSplash");
        var player = host.AddComponent<InterludeSplashPlayer>();
        player.Begin(documentRoot, textures);
    }

    void Begin(VisualElement documentRoot, Texture2D[] textures)
    {
        _textures = textures;
        _shownAt = Time.unscaledTime;

        _overlay = new VisualElement { name = "InterludeSplash" };
        _overlay.pickingMode = PickingMode.Position;
        _overlay.focusable = true;
        _overlay.style.position = Position.Absolute;
        _overlay.style.left = 0;
        _overlay.style.top = 0;
        _overlay.style.right = 0;
        _overlay.style.bottom = 0;
        _overlay.style.backgroundColor = Color.black;

        _frame = new VisualElement { name = "InterludeSplashFrame" };
        _frame.pickingMode = PickingMode.Ignore;
        _frame.style.position = Position.Absolute;
        _frame.style.left = 0;
        _frame.style.top = 0;
        _frame.style.right = 0;
        _frame.style.bottom = 0;
        _frame.style.backgroundColor = Color.black;
        _frame.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
        _overlay.Add(_frame);
        documentRoot.Add(_overlay);
        _overlay.BringToFront();
        _overlay.RegisterCallback<PointerDownEvent>(OnPointer);
        _overlay.RegisterCallback<KeyDownEvent>(OnKey);
        _overlay.Focus();

        ShowFrame(0);
        Debug.Log("[InterludeSplash] frame 1/" + Frames.Length + " " + Frames[0]);

        AudioClip clip = Resources.Load<AudioClip>("Data/Audio/intro");
        if (clip != null)
        {
            _music = gameObject.AddComponent<AudioSource>();
            _music.clip = clip;
            _music.loop = false;
            _music.spatialBlend = 0f;
            _music.playOnAwake = false;
            _music.Play();
        }
        else
        {
            Debug.LogWarning("[InterludeSplash] intro clip missing at Resources/Data/Audio/intro");
        }

        StartCoroutine(PlayFrames());
        StartCoroutine(CaptureProof());
    }

    void ShowFrame(int index)
    {
        Texture2D tex = _textures[index];
        if (tex == null)
            return;
        _frame.style.backgroundImage = new StyleBackground(tex);
        _overlay.BringToFront();
        Debug.Log("[InterludeSplash] showing " + Frames[index]);
    }

    IEnumerator PlayFrames()
    {
        int index = 0;
        while (!_dismissed)
        {
            float t = 0f;
            while (t < FrameSeconds && !_dismissed)
            {
                t += Time.unscaledDeltaTime;
                if (_overlay != null)
                    _overlay.BringToFront();
                yield return null;
            }

            if (_dismissed)
                break;

            index++;
            if (index >= _textures.Length)
                break;
            ShowFrame(index);
        }

        Dismiss();
    }

    IEnumerator CaptureProof()
    {
        string path = ReadShotPath();
        if (string.IsNullOrEmpty(path))
            yield break;

        for (int i = 0; i < 12; i++)
            yield return null;
        yield return new WaitForEndOfFrame();

        Texture2D tex = ScreenCapture.CaptureScreenshotAsTexture();
        if (tex == null)
        {
            Debug.LogWarning("[InterludeSplash] screenshot capture returned null");
            yield break;
        }

        byte[] png = tex.EncodeToPNG();
        Destroy(tex);
        string dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
        File.WriteAllBytes(path, png);
        Debug.Log("[InterludeSplash] wrote " + path + " bytes=" + png.Length);
    }

    static string ReadShotPath()
    {
        try
        {
            if (!File.Exists(ShotPathFile))
                return null;
            string path = File.ReadAllText(ShotPathFile).Trim();
            return string.IsNullOrEmpty(path) ? null : path;
        }
        catch (IOException)
        {
            return null;
        }
    }

    void Update()
    {
        if (_dismissed)
            return;
        if (Time.unscaledTime - _shownAt < SkipLockSeconds)
            return;
        if (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            _dismissed = true;
    }

    void OnPointer(PointerDownEvent evt)
    {
        if (Time.unscaledTime - _shownAt < SkipLockSeconds)
            return;
        _dismissed = true;
        evt.StopPropagation();
    }

    void OnKey(KeyDownEvent evt)
    {
        if (Time.unscaledTime - _shownAt < SkipLockSeconds)
            return;
        _dismissed = true;
        evt.StopPropagation();
    }

    void Dismiss()
    {
        if (_overlay != null && _overlay.parent != null)
            _overlay.RemoveFromHierarchy();
        _overlay = null;
        if (_music != null)
            _music.Stop();
        if (this != null)
            Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (_overlay != null && _overlay.parent != null)
            _overlay.RemoveFromHierarchy();
    }
}
