using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public sealed partial class InterludeSplashPlayer : MonoBehaviour
{
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
        if (PollInput.AnyKeyOrMouseDown)
            _dismissed = true;
        // Proof shot of the credentials window only. Normal play still waits out the splash.
        if (InterludeLoginProof.IsArmed)
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
