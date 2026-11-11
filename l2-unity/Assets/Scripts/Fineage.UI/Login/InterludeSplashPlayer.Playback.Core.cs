using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public sealed partial class InterludeSplashPlayer : MonoBehaviour
{
    static bool ShouldPlay()
    {
        try
        {
            if (File.Exists(ShotPathFile))
                return true;
        }
        catch (IOException)
        {
            /* play stays off */
        }

        string env = Environment.GetEnvironmentVariable("FINEAGE_SPLASH");
        if (env == "1" || string.Equals(env, "true", StringComparison.OrdinalIgnoreCase))
            return true;

        string[] args = Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-fineageSplash")
                return true;
        }

        return false;
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
        _frame.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
        _frame.style.backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Center);
        _frame.style.backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Center);
        _frame.style.backgroundRepeat = new BackgroundRepeat(Repeat.NoRepeat, Repeat.NoRepeat);
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
            _music.volume = GameSettings.Music;
            _music.Play();
            Debug.Log("[InterludeSplash] playing intro " + clip.length.ToString("0.0") + "s");
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
}
