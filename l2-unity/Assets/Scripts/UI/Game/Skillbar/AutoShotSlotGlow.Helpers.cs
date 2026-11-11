using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static partial class AutoShotSlotGlow
{
    public static IVisualElementScheduledItem StartWave(VisualElement glow)
    {
        if (glow == null)
            return null;

        EnsureFrames();
        if (_frames == null || _frames.Length == 0)
            return null;

        int index = 0;
        glow.style.backgroundImage = new StyleBackground(_frames[0]);
        return glow.schedule.Execute(() =>
        {
            if (glow.style.display == DisplayStyle.None)
                return;
            index = (index + 1) % _frames.Length;
            Texture2D frame = _frames[index];
            if (frame != null)
                glow.style.backgroundImage = new StyleBackground(frame);
        }).Every(FrameMs);
    }

    static void EnsureFrames()
    {
        if (_triedLoad)
            return;
        _triedLoad = true;

        var list = new List<Texture2D>(FrameCount);
        for (int i = 1; i <= FrameCount; i++)
        {
            string path = FrameRoot + i.ToString("00");
            Texture2D tex = Resources.Load<Texture2D>(path);
            if (tex != null)
                list.Add(tex);
        }

        _frames = list.Count > 0 ? list.ToArray() : null;
    }
}
